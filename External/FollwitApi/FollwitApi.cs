using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CookComputing.XmlRpc;
using Follwit.API.Data;
using System.Security.Cryptography;

namespace Follwit.API {
    public class FollwitApi {

        public static readonly string DefaultUrl = "http://follw.it/api/2/";

        public event FitAPIRequestDelegate RequestEvent;
        public event FitAPIResponseDelegate ResponseEvent;

        public delegate void FitAPIRequestDelegate(string RequestText);
        public delegate void FitAPIResponseDelegate(string ResponseText);

        public FitUser User {
            get;
            internal set;
        }

        public IFollwitProxy Proxy {
            get;
            internal set;
        }

        public string ApiUrl {
            get;
            internal set;
        }

        #region Login / Account Creation Methods

        /// <summary>
        /// Attempts to login with the given username and password.
        /// </summary>
        /// <returns>A FollwitAPI object if login is successful.</returns>
        public static FollwitApi Login(string username, string hashedPassword) {
            return Login(username, hashedPassword, DefaultUrl);
        }

        /// <summary>
        /// Attempts to login with the given username and password.
        /// </summary>
        /// <returns>A FollwitAPI object if login is successful.</returns>
        public static FollwitApi Login(string username, string hashedPassword, string apiUrl) {
            if (apiUrl == null) apiUrl = DefaultUrl;

            IFollwitProxy proxy = CreateProxy(username, hashedPassword, apiUrl);
            try { object result = proxy.CheckAuthentication(); }
            catch (XmlRpcServerException ex) {
                if (ex.Message == "Unauthorized") return null;
                else throw;
            }

            FollwitApi api = new FollwitApi(username, hashedPassword, apiUrl, proxy);
            api.GetUserData();

            proxy.RequestEvent += new XmlRpcRequestEventHandler(api.proxy_RequestEvent);
            proxy.ResponseEvent += new XmlRpcResponseEventHandler(api.proxy_ResponseEvent);

            return api;
        }

        public static bool CreateUser(string username, string password, string email, string locale, bool privateProfile) {
            return CreateUser(username, password, email, locale, privateProfile, DefaultUrl);
        }

        public static bool CreateUser(string username, string password, string email, string locale, bool privateProfile, string APIURL) {
            if (APIURL == null) APIURL = DefaultUrl;

            var proxy = CreateProxy(null, null, APIURL);
            try {
                proxy.CreateUser(username, password, email, locale, privateProfile);
            }
            catch (XmlRpcFaultException ex) {
                if (ex.FaultCode == 1) { // missing required field
                    string fieldname = ex.FaultString.Substring(0, ex.FaultString.IndexOf(" "));
                    throw new RequiredFieldMissingException(fieldname);
                }

                else if (ex.FaultCode == 2)
                    throw new UsernameAlreadyExistsException();

                else
                    throw;
            }

            return true;
        }

        public static bool IsUsernameAvailable(string username) {
            return IsUsernameAvailable(username, DefaultUrl);
        }

        public static bool IsUsernameAvailable(string username, string APIURL) {
            if (APIURL == null) APIURL = DefaultUrl;
            var proxy = CreateProxy(null, null, APIURL);

            var xmlData = (XmlRpcStruct)proxy.CheckUsernameAvailability(username);
            return bool.Parse(xmlData["available"].ToString());
        }

        public static string HashPassword(string password) {
            // salt + hash
            string salt = "52c3a0d0-f793-46fb-a4c0-35a0ff6844c8";
            string saltedPassword = password + salt;
            SHA1CryptoServiceProvider sha1Obj = new SHA1CryptoServiceProvider();
            byte[] bHash = sha1Obj.ComputeHash(Encoding.ASCII.GetBytes(saltedPassword));
            string sHash = "";
            foreach (byte b in bHash)
                sHash += b.ToString("x2");
            return sHash;
        }

        #endregion

        #region User Account Methods


        /// <summary>
        /// Updates a user profile at follw.it.  Any fields left blank are not updated
        /// </summary>
        /// <param name="email"></param>
        /// <param name="locale"></param>
        /// <param name="privateProfile"></param>
        /// <returns></returns>
        public bool UpdateUser(string email, string locale, bool? privateProfile) {
            if (email == null) email = "";
            if (locale == null) locale = "";
            if (privateProfile == null) privateProfile = User.PrivateProfile;
            Proxy.UpdateUser(email, locale, (bool)privateProfile);
            return true;
        }


        public void GetUserData() {
            if (User == null)
                return;

            XmlRpcStruct userData = (XmlRpcStruct)Proxy.GetUserData();
            User.Name = userData["Username"].ToString();
            User.Email = userData["Email"].ToString();
            User.PrivateProfile = (userData["PrivateProfile"].ToString() == "1");
            User.PrivateUrl = userData["PrivateURL"].ToString();
            User.Locale = userData["Locale"].ToString();
            User.AdultMoviesVisible = userData["AdultMovies"].ToString() == "1";
            User.LastSeen = DateTime.Parse(userData["LastSeen"].ToString());
        }

        #endregion

        #region Movie Methods

        /// <summary>
        /// Adds a collection of new movies with data to follw.it, and adds it to the user's collection.
        /// </summary>
        public void AddMoviesToCollection(ref List<FitMovie> movies) {
            var movieIds = Proxy.AddMoviesToCollectionWithData(movies.ToArray());

            // insert the MpsId's into the original list
            foreach (XmlRpcStruct movieId in movieIds) {
                FitMovie movieDTO = movies.Find(
                    delegate(FitMovie a) {
                        return a.InternalId == Convert.ToInt32(movieId["InternalId"]);
                    });

                if (movieDTO != null) {
                    movieDTO.MovieId = Convert.ToInt32(movieId["MovieId"]);
                    movieDTO.UserRating = Convert.ToInt32(movieId["UserRating"]);
                    movieDTO.Watched = Convert.ToString(movieId["Watched"]) == "1";
                }
            }
        }

        /// <summary>
        /// Adds a new movie with data to follw.it, and adds it to the user's collection.
        /// </summary>
        public void AddMoviesToCollection(ref FitMovie movie) {
            List<FitMovie> movies = new List<FitMovie>() { movie };
            AddMoviesToCollection(ref movies);
        }



        /// <summary>
        /// Remove a movie from an user's collection
        /// </summary>
        /// <param name="MovieId"></param>
        /// <returns></returns>
        public bool RemoveMovieFromCollection(int movieId) {
            Proxy.RemoveMovieFromCollection(movieId);
            return true;
        }

        public bool UploadCover(int movieId, string file) {
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

            byte[] binaryData = new Byte[fileStream.Length];
            long bytesRead = fileStream.Read(binaryData, 0, (int)fileStream.Length);
            fileStream.Close();

            string base64String =
                System.Convert.ToBase64String(binaryData, 0, binaryData.Length);

            Proxy.UploadCover(movieId, base64String);

            return true;
        }

        public List<TaskListItem> GetUserTaskList(DateTime? startDate, out DateTime serverTime) {
            List<TaskListItem> result = new List<TaskListItem>();
            serverTime = DateTime.MinValue;

            var tasks = Proxy.GetUserTaskList(startDate);
            foreach (XmlRpcStruct task in tasks) {
                TaskListItem tli = new TaskListItem();
                switch (task["ItemType"].ToString()) {
                    case "server_time":
                        DateTime.TryParse(task["ServerTime"].ToString(), out serverTime);
                        break;
                    case "cover_request":
                        tli.Task = TaskItemType.CoverRequest;
                        tli.MovieId = (int)task["MovieId"];
                        result.Add(tli);
                        break;
                    case "new_rating":
                        tli.Task = TaskItemType.NewRating;
                        tli.MovieId = (int)task["MovieId"];
                        tli.Rating = (int)task["Rating"];
                        result.Add(tli);
                        break;
                    case "new_watched_status":
                        tli.Task = TaskItemType.NewWatchedStatus;
                        tli.MovieId = (int)task["MovieId"];
                        tli.Watched = ((string)task["Watched"]) == "1";
                        result.Add(tli);
                        break;
                    case "updated_movie_id":
                        tli.Task = TaskItemType.UpdatedMovieId;
                        tli.MovieId = (int)task["MovieId"];
                        tli.NewMovieId = (int)task["NewMovieId"];
                        result.Add(tli);
                        break;
                    case "new_series_rating":
                        tli.Task = TaskItemType.NewSeriesRating;
                        tli.SeriesId = (int)task["SeriesId"];
                        tli.TvdbSeriesId = (int)task["TvdbSeriesId"];
                        tli.Rating = (int)task["Rating"];
                        result.Add(tli);
                        break;
                    case "new_episode_rating":
                        tli.Task = TaskItemType.NewEpisodeRating;
                        tli.EpisodeId = (int)task["EpisodeId"];
                        tli.Rating = (int)task["Rating"];
                        result.Add(tli);
                        break;
                    case "new_episode_watched_status":
                        tli.Task = TaskItemType.NewEpisodeWatchedStatus;
                        tli.EpisodeId = (int)task["EpisodeId"];
                        tli.Watched = ((string)task["Watched"]) == "1";
                        result.Add(tli);
                        break;
                    
                    default:
                        break;
                }
            }

            return result;
        }

        public void SetMovieRating(int movieId, int rating) {
            checkValidRatingValueProvided(rating);
            Proxy.SetMovieRating(movieId, rating.ToString());
        }

        public void WatchMovie(int movieId, bool insertIntoStream) {
            Proxy.WatchMovie(movieId, insertIntoStream);
        }

        public void UnwatchMovie(int movieId) {
            Proxy.UnwatchMovie(movieId);
        }

        public void WatchingMovie(int movieId) {
            Proxy.WatchingMovie(movieId);
        }

        public void StopWatchingMovie(int movieId) {
            Proxy.StopWatchingMovie(movieId);
        }

        #endregion

        #region TV Show Methods

        /// <summary>
        /// Sets the Watched Status of an Episode
        /// </summary>
        /// <param name="SourceName">Name of the source used to identify the episode (ex: follwit or tvdb)</param>
        /// <param name="SourceId">Identity of the episode at the remote source (ex: follwit episode id, or thetvdb episode id)</param>
        /// <param name="Watched">The Watched Status of the Episode</param>
        /// <param name="InsertIntoStream">If Watched is True, InsertIntoStream indicates should be true if the episode just finished playing. Otherwise ignored</param>
        public void WatchedTVEpisode(string SourceName, string SourceId, bool Watched, bool InsertIntoStream) {
            if (string.IsNullOrEmpty(SourceId) || string.IsNullOrEmpty(SourceName))
                return;

            Proxy.WatchedTVEpisode(SourceName, SourceId, Watched, InsertIntoStream);
        }

        /// <summary>
        /// Notifies follw.it that an Episode is playing right now. Should be followed by a StopWatchingEpisode call. Live Watching status expires after 60 minutes.
        /// </summary>
        /// <param name="SourceName">Name of the source used to identify the episode (ex: follwit or tvdb)</param>
        /// <param name="SourceId">Identity of the episode at the remote source (ex: follwit episode id, or thetvdb episode id)</param>
        public void WatchingTVEpisode(string SourceName, string SourceId) {
            Proxy.WatchingTVEpisode(SourceName, SourceId);
        }

        /// <summary>
        /// Notifies follw.it that an Episode finished playing. If not follwing a WatchingTVEpisode the call is ignored.
        /// </summary>
        /// <param name="SourceName">Name of the source used to identify the episode (ex: follwit or tvdb)</param>
        /// <param name="SourceId">Identity of the episode at the remote source (ex: follwit episode id, or thetvdb episode id)</param>
        public void StopWatchingTVEpisode(string SourceName, string SourceId) {
            Proxy.StopWatchingTVEpisode(SourceName, SourceId);
        }

        /// <summary>
        /// Rates a single episode
        /// </summary>
        /// <param name="SourceName">Name of the source used to identify the episode (ex: follwit or tvdb)</param>
        /// <param name="SourceId">Identity of the episode at the remote source (ex: follwit episode id, or thetvdb episode id)</param>
        /// <param name="Rating">The user provided rating (1-5), higher being better, 0 to remove the rating</param>
        public void RateTVEpisode(string SourceName, string SourceId, int Rating) {
            checkValidRatingValueProvided(Rating);
            Proxy.RateTVEpisode(SourceName, SourceId, Rating);
        }

        /// <summary>
        /// Rates a single series
        /// </summary>
        /// <param name="SourceName">Name of the source used to identify the series (ex: follwit or tvdb)</param>
        /// <param name="SourceId">Identity of the series at the remote source (ex: follwit series id, or thetvdb series id)</param>
        /// <param name="Rating">The user provided rating (1-5), higher being better, 0 to remove the rating</param>
        public void RateTVSeries(string SourceName, string SourceId, int Rating) {
            Proxy.RateTVSeries(SourceName, SourceId, Rating);
        }

        /// <summary>
        /// Performs a full sync on a single episodes
        /// </summary>
        /// <param name="episode">A BulkEpisode object holding episode information to sync</param>
        public FitEpisode BulkAction(FitEpisode episode) {
            return BulkAction(new[] { episode })[0];
        }

        /// <summary>
        /// Performs a full sync on a sequence of episodes
        /// </summary>
        /// <param name="episodes">A sequence of episodes</param>
        public List<FitEpisode> BulkAction(IEnumerable<FitEpisode> episodes) {
            var episodesArray = episodes.ToArray();
            episodesArray.All(e => !e.Rating.HasValue || checkValidRatingValueProvided(e.Rating.Value));

            List<FitEpisode> rtn = new List<FitEpisode>();
            foreach (XmlRpcStruct currRecord in Proxy.BulkAction(episodesArray)) {
                FitEpisode currEp = new FitEpisode();
                currEp.FollwitId = (string) currRecord["EpisodeId"];
                currEp.SourceName = (string)currRecord["SourceName"];
                currEp.SourceId = (string)currRecord["SourceId"];
                currEp.Rating = int.Parse((string)currRecord["Rating"]);
                currEp.Watched = currRecord["Watched"].Equals("1");

                rtn.Add(currEp);
            }

            return rtn;
        }

        #endregion

        #region Private Methods

        private FollwitApi(string username, string hashedPassword, string apiUrl, IFollwitProxy proxy) {
            User = new FitUser();
            User.Name = username;
            User.HashedPassword = hashedPassword;

            ApiUrl = apiUrl;
            Proxy = proxy;
        }

        private static IFollwitProxy CreateProxy(string username, string password, string apiUrl) {
            if (apiUrl == null) apiUrl = DefaultUrl;

            IFollwitProxy proxy = XmlRpcProxyGen.Create<IFollwitProxy>();
            proxy.Url = apiUrl;
            proxy.UserAgent = "follw.it C# API";
            proxy.KeepAlive = false;

            if (username != null && username.Length > 0 && password != null && password.Length > 0) {
                string auth = username + ":" + password;
                auth = Convert.ToBase64String(Encoding.Default.GetBytes(auth));
                proxy.Headers["Authorization"] = "Basic " + auth;
            }

            return proxy;
        }

        static bool checkValidRatingValueProvided(int rating) {
            if (rating < 0 || rating > 5)
                throw new ArgumentOutOfRangeException("Rating", rating, "Valid Rating values are 1-5, higher being better, 0 to remove the rating");
            return true;
        }

        void proxy_RequestEvent(object sender, XmlRpcRequestEventArgs args) {
            if (RequestEvent != null) {
                TextReader txt = new StreamReader(args.RequestStream);
                string request = txt.ReadToEnd();
                RequestEvent(request);
            }
        }

        void proxy_ResponseEvent(object sender, XmlRpcResponseEventArgs args) {
            if (ResponseEvent != null) {
                TextReader txt = new StreamReader(args.ResponseStream);
                string response = txt.ReadToEnd();
                ResponseEvent(response);
            }
        }

        #endregion
    }

    #region Custom Exceptions

    public class RequiredFieldMissingException : Exception {
        public string FieldName { get; set; }
        public RequiredFieldMissingException(string fieldName) {
            this.FieldName = fieldName;
        }
    }

    public class MovieDoesNotExistException : MediaDoesNotExistException { }
    public class ShowDoesNotExistException : MediaDoesNotExistException { }
    public class MediaDoesNotExistException : Exception { }

    public class UsernameAlreadyExistsException : Exception { }

    #endregion

    class APIHttpResponse {
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseString { get; set; }
    }

    public struct TaskListItem {
        public TaskItemType Task;
        public int MovieId;
        public int SeriesId;
        public int TvdbSeriesId;
        public int EpisodeId;
        public int? Rating;
        public bool? Watched;
        public int? NewMovieId;
    }

    public enum TaskItemType {
        CoverRequest,
        NewRating,
        NewWatchedStatus,
        UpdatedMovieId,
        NewSeriesRating,
        NewEpisodeRating,
        NewEpisodeWatchedStatus
    }
}

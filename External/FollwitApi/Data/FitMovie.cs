﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Follwit.API.Data {
    public class FitMovie {

        /// <summary>
        /// Id of the movie in your own internal system. 
        /// This will be passed back from some follw.it bulk operations, for easier identification
        /// </summary>
        public int InternalId { 
            get { return _internalId; }
            set { _internalId = value; }
        } private int _internalId = 0;

        /// <summary>
        /// Id of the movie at follw.it
        /// </summary>
        public int MovieId {
            get { return _movieId; }
            set { _movieId = value; }
        } private int _movieId = 0;

        /// <summary>
        /// Pipe delimited list of name=value pairs the third party resources for the movie (ex: imdb.com=tt000000)
        /// </summary>
        public string Resources { get; set; }
        public string FileHash {
            get { return _fileHash; }
            set { _fileHash = value; }
        } private string _fileHash = "";

        public string Title { get; set; }
        public string Year { get; set; }
        public string Certification { get; set; }
        public string Language { get; set; }
        public string Tagline { get; set; }
        public string Summary { get; set; }
        public string Score { get; set; }
        public string Popularity { get; set; }
        public string Runtime { get; set; }
        /// <summary>
        /// Pipe delimited list of genres
        /// </summary>
        public string Genres { get; set; }
        /// <summary>
        /// Pipe delimited list of directors
        /// </summary>
        public string Directors { get; set; }
        /// <summary>
        /// Pipe delimited list of writers
        /// </summary>
        public string Writers { get; set; }
        /// <summary>
        /// Pipe delimited list of actors
        /// </summary>
        public string Cast { get; set; }
        public string TranslatedTitle { get; set; }
        public string Locale { get; set; }

        public bool Watched { get; set; }
        public DateTime LastWatchDate { get; set; }
        public int UserRating { get; set; }

    }
}

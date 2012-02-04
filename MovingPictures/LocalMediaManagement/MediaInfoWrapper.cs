using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using CookComputing.XmlRpc;
using Cornerstone.Extensions.IO;
using MediaPortal.Player;
using System.IO;
using NLog;
using System.Text.RegularExpressions;
using System.Linq;

#region API

# endregion

namespace MediaPortal.Plugins.MovingPictures.LocalMediaManagement
{
    /// <summary>
    /// This class is here to temporarily replace the MediaInfoWrapper included with MediaPortal
    /// Once MP has the new changes, this file can be deleted.
    /// </summary>
  public class MediaInfoWrapper
  {



    #region private vars

    private static Logger logger = LogManager.GetCurrentClassLogger();

    private MediaInfo _mI = null;

    private double _framerate = 0;
    private int _width = 0;
    private int _height = 0;
    private int _audioRate = 0;
    private int _audioChannels = 0;
    private int _numSubtitles = 0;
    private int _multiview_count = 0;
    private string _aspectRatio = "";
    private string _videoCodec = string.Empty;
    private string _multiview_layout = string.Empty;
    private string _audioCodec = string.Empty;
    private string _audioFormatProfile = string.Empty;
    private readonly List<AudioInfo> _audioInfo = new List<AudioInfo>();
    private readonly List<SubtitleInfo> _subtitleInfo = new List<SubtitleInfo>();
    private string _scanType = string.Empty;
    private bool _isDIVX = false; // mpeg4 DivX
    private bool _isXVID = false; // mpeg4 asp
    private bool _isH264 = false; // mpeg4 avc h264/x264
    private bool _isMP1V = false; // mpeg1 video (VCD)
    private bool _isMP2V = false; // mpeg2 video
    private bool _isMP4V = false; // mpeg4 generic
    private bool _isWMV = false;  // WMV 7-9
    private bool _is720P = false; // is 1280x720 video
    private bool _is1080P = false; // is 1980x1080 video, progressive
    private bool _is1080I = false; // is 1920x1080 video, interlaced
    private bool _isInterlaced = false; // is interlaced
    private bool _isHDTV = false; // is HDTV resolution
    private bool _isSDTV = false; // is SDTV resolution
    private bool _isAC3 = false;  // AC3
    private bool _isMP3 = false;  // MPEG-1 Audio layer 3
    private bool _isMP2A = false; // MPEG-1 Audio layer 2
    private bool _isDTS = false;  // DTS
    private bool _isOGG = false;  // OGG VORBIS
    private bool _isAAC = false;  // AAC
    private bool _isWMA = false;  // Windows Media Audio
    private bool _isPCM = false;  // RAW audio
    private bool _isTrueHD = false;  // TrueHD audio
    private bool _isDTSHD = false;  // DTSHD audio
    
    /// <summary>Is this a 3D Movie?</summary>
    private bool _is3D = false;
    private int _duration = 0;

    private bool _hasSubtitles = false;
    private bool _hasChapters = false;
    private static List<string> _subTitleExtensions = new List<string>();

    #endregion

    #region ctor's

    public MediaInfoWrapper(string strFile)
    {

      bool isTV = Util.Utils.IsLiveTv(strFile);
      bool isRadio = Util.Utils.IsLiveRadio(strFile);
      bool isDVD = Util.Utils.IsDVD(strFile);
      bool isVideo = Util.Utils.IsVideo(strFile);
      bool isAVStream = Util.Utils.IsAVStream(strFile); //rtsp users for live TV and recordings.

      if (isTV || isRadio || isAVStream)
      {
        return;
      }

      try
      {
        _mI = new MediaInfo();
        _mI.Open(strFile);

        FileInfo fileInfo = strFile.PathToFileInfo();
        DriveInfo driveInfo = fileInfo.GetDriveInfo();

        NumberFormatInfo providerNumber = new NumberFormatInfo();
        providerNumber.NumberDecimalSeparator = ".";

        double.TryParse(_mI.Get(StreamKind.Video, 0, "FrameRate"), NumberStyles.AllowDecimalPoint, providerNumber, out _framerate);
        _videoCodec = _mI.Get(StreamKind.Video, 0, "Codec").ToLower();
        _scanType = _mI.Get(StreamKind.Video, 0, "ScanType").ToLower();
        int.TryParse(_mI.Get(StreamKind.Video, 0, "Width"), out _width);
        int.TryParse(_mI.Get(StreamKind.Video, 0, "Height"), out _height);
        int.TryParse(_mI.Get(StreamKind.General, 0, "TextCount"), out _numSubtitles);
        int.TryParse(_mI.Get(StreamKind.Video, 0, "MultiView_Count"), out _multiview_count);
        _multiview_layout = _mI.Get(StreamKind.Video, 0, "MultiView_Layout");

        string aspectStr = _mI.Get(StreamKind.Video, 0, "AspectRatio/String");
        if (aspectStr == "4/3" || aspectStr == "4:3")
            _aspectRatio = "fullscreen";
        else
            _aspectRatio = "widescreen";
        
        if (strFile.ToLower().EndsWith(".ifo") && driveInfo != null && driveInfo.IsOptical()) {
            // mediainfo is not able to obtain duration of IFO files
            // so we use this to loop through all corresponding VOBs and add up the duration
            // we do not do this for optical drives because there are issues with some discs
            // taking more than 2 minutes(copy protection?)
            _duration = 0;
            string filePrefix = Path.GetFileName(strFile);
            filePrefix = filePrefix.Substring(0, filePrefix.LastIndexOf('_'));
            MediaInfo mi = new MediaInfo();
            foreach (string file in Directory.GetFiles(Path.GetDirectoryName(strFile), filePrefix + "*.VOB")) {
                mi.Open(file);
                int durationPart = 0;
                int.TryParse(_mI.Get(StreamKind.Video, 0, "PlayTime"), out durationPart);
                _duration += durationPart;
            }
        }
        else {
            int.TryParse(_mI.Get(StreamKind.Video, 0, "PlayTime"), out _duration);
        }

        _isInterlaced = (_scanType.IndexOf("interlaced") > -1);

        if (_height >= 720)
        {
          _isHDTV = true;
        }
        else
        {
          _isSDTV = true;
        }

        if ((_width == 1280 || _height == 720) && !_isInterlaced)
        {
          _is720P = true;
        }

        if ((_width == 1920 || _height == 1080) && !_isInterlaced)
        {
          _is1080P = true;
        }

        if ((_width == 1920 || _height == 1080) && _isInterlaced)
        {
          _is1080I = true;
        }

        _isDIVX = (_videoCodec.IndexOf("dx50") > -1) | (_videoCodec.IndexOf("div3") > -1); // DivX 5 and DivX 3
        _isXVID = (_videoCodec.IndexOf("xvid") > -1);
        _isH264 = (_videoCodec.IndexOf("avc") > -1 || _videoCodec.IndexOf("h264") > -1);
        _isMP1V = (_videoCodec.IndexOf("mpeg-1v") > -1);
        _isMP2V = (_videoCodec.IndexOf("mpeg-2v") > -1);
        _isMP4V = (_videoCodec.IndexOf("fmp4") > -1); // add more
        _isWMV = (_videoCodec.IndexOf("wmv") > -1); // wmv3 = WMV9

        _is3D = (_multiview_count > 1); // Probably 3D

        // missing cvid etc

        if (checkHasExternalSubtitles(strFile))
        {
            _hasSubtitles = true;
        } 
        else if (_numSubtitles > 0)
        {
            _hasSubtitles = true;
        }
        else
        {
            _hasSubtitles = false;
        }

        logger.Debug("MediaInfoWrapper: inspecting media : {0}", strFile);
        logger.Debug("MediaInfoWrapper: --- Video Information ---");
        logger.Debug("MediaInfoWrapper: FrameRate : {0}", _framerate);
        logger.Debug("MediaInfoWrapper: VideoCodec : {0}", _videoCodec);
        if (_isDIVX) logger.Debug("MediaInfoWrapper: IsDIVX: {0}", _isDIVX);
        if (_isXVID) logger.Debug("MediaInfoWrapper: IsXVID: {0}", _isXVID);
        if (_isH264) logger.Debug("MediaInfoWrapper: IsH264: {0}", _isH264);
        if (_isMP1V) logger.Debug("MediaInfoWrapper: IsMP1V: {0}", _isMP1V);
        if (_isMP2V) logger.Debug("MediaInfoWrapper: IsMP2V: {0}", _isMP2V);
        if (_isMP4V) logger.Debug("MediaInfoWrapper: IsMP4V: {0}", _isMP4V);
        if (_isWMV) logger.Debug("MediaInfoWrapper: IsWMV: {0}", _isWMV);

        logger.Debug("MediaInfoWrapper: HasSubtitles : {0}", _hasSubtitles);
        logger.Debug("MediaInfoWrapper: NumSubtitles : {0}", _numSubtitles);
        logger.Debug("MediaInfoWrapper: Scan type : {0}", _scanType);
        logger.Debug("MediaInfoWrapper: IsInterlaced: {0}", _isInterlaced);
        logger.Debug("MediaInfoWrapper: Width : {0}", _width);
        logger.Debug("MediaInfoWrapper: Height : {0}", _height);
        logger.Debug("MediaInfoWrapper: AspectRatio : {0}", _aspectRatio);
        logger.Debug("");

        // Retrieve Audio Information
        GetAudioInformation();

        // Retrieve Subtitle Information
        GetSubtitleInformation();

        int numMenus;
        if ((int.TryParse(_mI.Get(StreamKind.General, 0, "MenuCount"), out numMenus)) && numMenus > 0) _hasChapters = true;
        logger.Debug("MediaInfoWrapper: Has Chapters: {0}", _hasChapters);

      }
      catch (Exception ex)
      {
        logger.Error("MediaInfo processing failed ('MediaInfo.dll' may be missing): {0}", ex.Message);
      }
      finally
      {
        if (_mI != null)
        {
          _mI.Close();
        }
      }
    }

    #endregion

    #region internal methods

    /// <summary>
    /// Retrieve friendly Audio Channels Name
    /// </summary>
    /// <param name="channels">Channels</param>
    /// <returns></returns>
    internal static string GetAudioChannelsFriendly(int channels)
    {
        switch (channels)
        {
            case 8:
                return "7.1";
            case 6:
                return "5.1";
            case 2:
                return "stereo";
            case 1:
                return "mono";
            default:
                return channels.ToString();
        }
    }

    #endregion

    #region private methods

    /// <summary>
    /// Retrieve information about Audio Streams
    /// </summary>
    private void GetAudioInformation()
    {
        logger.Debug("MediaInfoWrapper: --- Audio Information ---");
        int intValue;
        int iAudioStreams = _mI.Count_Get(StreamKind.Audio);
        for (int i = 0; i < iAudioStreams; i++)
        {
            string sChannels = Regex.Split(_mI.Get(StreamKind.Audio, i, "Channel(s)"), @"\D+").Max();

            if (int.TryParse(sChannels, out intValue))
            {
                var audioInfo = new AudioInfo { AudioChannels = intValue };
                if (int.TryParse(_mI.Get(StreamKind.Audio, i, "SamplingRate"), out _audioRate)) { audioInfo.AudioRate = _audioRate; }
                audioInfo.AudioCodecOriginal = _mI.Get(StreamKind.Audio, i, "Codec/String").ToLower();
                audioInfo.AudioFormatProfile = _mI.Get(StreamKind.Audio, i, "Format_Profile").ToLower();
                audioInfo.AudioLanguage = _mI.Get(StreamKind.Audio, i, "Language");
                audioInfo.AudioLanguageMore = _mI.Get(StreamKind.Audio, i, "Language_More");
                audioInfo.AudioStreamIndex = i;
                audioInfo.AudioTitle = _mI.Get(StreamKind.Audio, i, "Title");
                _audioInfo.Add(audioInfo);

                logger.Debug("MediaInfoWrapper: AudioStreamIndex: {0}", audioInfo.AudioStreamIndex);
                logger.Debug("MediaInfoWrapper: AudioCodec: {0}", audioInfo.AudioCodecOriginal);
                logger.Debug("MediaInfoWrapper: AudioRate: {0}", audioInfo.AudioRate);
                logger.Debug("MediaInfoWrapper: AudioChannels: {0}", audioInfo.AudioChannels);
                logger.Debug("MediaInfoWrapper: AudioFormatProfile: {0}", audioInfo.AudioFormatProfile);
                logger.Debug("MediaInfoWrapper: AudioLanguage: {0}", audioInfo.AudioLanguage);
                logger.Debug("MediaInfoWrapper: AudioLanguageMore: {0}", audioInfo.AudioLanguageMore);
                logger.Debug("MediaInfoWrapper: AudioTitle: {0}", audioInfo.AudioTitle);

                if (audioInfo.IsAC3)
                    logger.Debug("MediaInfoWrapper: IsAC3 : {0}", audioInfo.IsAC3);
                if (audioInfo.IsMP3)
                    logger.Debug("MediaInfoWrapper: IsMP3 : {0}", audioInfo.IsMP3);
                if (audioInfo.IsMP2A)
                    logger.Debug("MediaInfoWrapper: IsMP2A: {0}", audioInfo.IsMP2A);
                if (audioInfo.IsDTS)
                    logger.Debug("MediaInfoWrapper: IsDTS : {0}", audioInfo.IsDTS);
                if (audioInfo.IsTrueHD)
                    logger.Debug("MediaInfoWrapper: IsTrueHD : {0}", audioInfo.IsTrueHD);
                if (audioInfo.IsDTSHD)
                    logger.Debug("MediaInfoWrapper: IsDTSHD : {0}", audioInfo.IsDTSHD);
                if (audioInfo.IsOGG)
                    logger.Debug("MediaInfoWrapper: IsOGG : {0}", audioInfo.IsOGG);
                if (audioInfo.IsAAC)
                    logger.Debug("MediaInfoWrapper: IsAAC : {0}", audioInfo.IsAAC);
                if (audioInfo.IsWMA)
                    logger.Debug("MediaInfoWrapper: IsWMA: {0}", audioInfo.IsWMA);
                if (audioInfo.IsPCM)
                    logger.Debug("MediaInfoWrapper: IsPCM: {0}", audioInfo.IsPCM);

                logger.Debug("");
            }
        }

        FillDefaultAudioInformation();
    }

    /// <summary>
    /// Provide default Audio Information, for compatibility with the "old" Audio information method
    /// </summary>
    private void FillDefaultAudioInformation()
    {
        // Get "best" stream (most channels)
        var maxAudioChannels = _audioInfo.Max(channels => channels.AudioChannels);
        var bestAudioStream = _audioInfo.Where(info => info.AudioChannels == maxAudioChannels).First();

        _audioChannels = bestAudioStream.AudioChannels;
        _audioCodec = bestAudioStream.AudioCodec;
        _audioFormatProfile = bestAudioStream.AudioFormatProfile;
        _audioRate = bestAudioStream.AudioRate;

        _isAC3 = bestAudioStream.IsAC3;
        _isMP3 = bestAudioStream.IsMP3;
        _isMP2A = bestAudioStream.IsMP2A;
        _isDTS = bestAudioStream.IsDTS;
        _isTrueHD = bestAudioStream.IsTrueHD;
        _isDTSHD = bestAudioStream.IsDTSHD;
        _isOGG = bestAudioStream.IsOGG;
        _isAAC = bestAudioStream.IsAAC;
        _isWMA = bestAudioStream.IsWMA;
        _isPCM = bestAudioStream.IsPCM;
    }

    /// <summary>
    /// Retrieve Information about Subtitles
    /// </summary>
    private void GetSubtitleInformation()
    {
        logger.Debug("MediaInfoWrapper: --- Subtitle Information ---");
        int intValue;
        int iTextStreams = _mI.Count_Get(StreamKind.Text);
        for (int i = 0; i < iTextStreams; i++)
        {
            var subtitleInfo = new SubtitleInfo
                                 {
                                   Language = _mI.Get(StreamKind.Text, i, "Language"),
                                   Internal = true
                                 };

            if (string.IsNullOrEmpty(subtitleInfo.Language)) continue;
            _subtitleInfo.Add(subtitleInfo);

            logger.Debug("MediaInfoWrapper: Language: {0}", subtitleInfo.Language);
            logger.Debug("MediaInfoWrapper: Internal: {0}", subtitleInfo.Internal);
            
            logger.Debug("");
        }
    }

    private bool checkHasExternalSubtitles(string strFile)
    {
      if (_subTitleExtensions.Count == 0)
      {
        // load them in first time
        _subTitleExtensions.Add(".aqt");
        _subTitleExtensions.Add(".asc");
        _subTitleExtensions.Add(".ass");
        _subTitleExtensions.Add(".dat");
        _subTitleExtensions.Add(".dks");
        _subTitleExtensions.Add(".js");
        _subTitleExtensions.Add(".jss");
        _subTitleExtensions.Add(".lrc");
        _subTitleExtensions.Add(".mpl");
        _subTitleExtensions.Add(".ovr");
        _subTitleExtensions.Add(".pan");
        _subTitleExtensions.Add(".pjs");
        _subTitleExtensions.Add(".psb");
        _subTitleExtensions.Add(".rt");
        _subTitleExtensions.Add(".rtf");
        _subTitleExtensions.Add(".s2k");
        _subTitleExtensions.Add(".sbt");
        _subTitleExtensions.Add(".scr");
        _subTitleExtensions.Add(".smi");
        _subTitleExtensions.Add(".son");
        _subTitleExtensions.Add(".srt");
        _subTitleExtensions.Add(".ssa");
        _subTitleExtensions.Add(".sst");
        _subTitleExtensions.Add(".ssts");
        _subTitleExtensions.Add(".stl");
        _subTitleExtensions.Add(".sub");
        _subTitleExtensions.Add(".txt");
        _subTitleExtensions.Add(".vkt");
        _subTitleExtensions.Add(".vsf");
        _subTitleExtensions.Add(".zeg");

      }
      string filenameNoExt = System.IO.Path.GetFileNameWithoutExtension(strFile);
      try
      {
        foreach (string file in System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(strFile), filenameNoExt + "*"))
        {
          System.IO.FileInfo fi = new System.IO.FileInfo(file);
          if (_subTitleExtensions.Contains(fi.Extension.ToLower())) return true;
        }
      }
      catch (Exception)
      {
        // most likley path not available
      }

      return false;
    }

    

    #endregion

    #region public video related properties

    public string AspectRatio
    {
      get { return _aspectRatio; }
    }

    public string VideoCodec
    {
      get { 
          string tempCodec = String.Empty;
          if (_isDIVX)
              tempCodec = "DIVX";
          else if (_isXVID)
              tempCodec = "XVID";
          else if (_isH264)
              tempCodec = "H264";
          else if (_isMP1V)
              tempCodec = "MP1V";
          else if (_isMP2V)
              tempCodec = "MP2V";
          else if (_isWMV)
              tempCodec = "WMV";
          else
              tempCodec = _videoCodec;

          return tempCodec; 
      }
    }

    public string VideoResolution
    {
        get
        {
            if (Is1080P)
                return "1080p";
            else if (Is1080I)
                return "1080i";
            else if (Is720P)
                return "720p";
            else if (IsHDTV)
                return "HD";
            else
                return "SD";
        }
    }

    public double Framerate
    {
      get { return _framerate; }
    }

    public bool IsDIVX
    {
      get { return _isDIVX; }
    }

    public bool IsXVID
    {
      get { return _isXVID; }
    }

    public bool IsH264
    {
      get { return _isH264; }
    }

    public bool IsMP1V
    {
      get { return _isMP1V; }
    }

    public bool IsMP2V
    {
      get { return _isMP2V; }
    }

    public bool IsMP4V
    {
      get { return _isMP4V; }
    }

    public bool IsWMV
    {
      get { return _isWMV; }
    }

    public bool Is720P
    {
      get { return _is720P; }
    }

    public bool Is1080P
    {
      get { return _is1080P; }
    }

    public bool Is1080I
    {
      get { return _is1080I; }
    }

    public bool IsHDTV
    {
      get { return _isHDTV; }
    }

    public bool IsSDTV
    {
      get { return _isSDTV; }
    }

    /// <summary>Movie is 3D</summary>
    public bool Is3D
    {
      get { return _is3D; }
    }

    public bool IsInterlaced
    {
      get { return _isInterlaced; }
    }

    public int Width
    {
      get { return _width; }
    }

    public int Height
    {
      get { return _height; }
    }

    #endregion

    #region public audio related properties

    public IEnumerable<AudioInfo> AudioInfo
    {
        get { return _audioInfo; }
    }

    public string AudioCodec
    {
      get {
          string tempCodec = String.Empty;
          if (_isTrueHD)
              tempCodec = "TrueHD";
          else if (_isDTSHD)
              tempCodec = "DTSHD";
          else if (_isAC3)
              tempCodec = "AC3";
          else if (_isMP3)
              tempCodec = "MP3";
          else if (_isMP2A)
              tempCodec = "MP2A";
          else if (_isDTS)
              tempCodec = "DTS";
          else if (_isOGG)
              tempCodec = "VORBIS";
          else if (_isAAC)
              tempCodec = "AAC";
          else if (_isWMA)
              tempCodec = "WMA";
          else if (_isPCM)
              tempCodec = "PCM";
          else
              tempCodec = _audioCodec;
          return tempCodec; 
      }
    }

    public int AudioRate
    {
      get { return _audioRate; }
    }

    public int AudioChannels
    {
      get { return _audioChannels; }
    }

    public string AudioChannelsFriendly
    {
        get
        {
            return GetAudioChannelsFriendly(AudioChannels);
        }
    }

    public bool IsAC3
    {
      get { return _isAC3; }
    }

    public bool IsMP3
    {
      get { return _isMP3; }
    }

    public bool IsMP2A
    {
      get { return _isMP2A; }
    }

    public bool IsWMA
    {
      get { return _isWMA; }
    }

    public bool IsPCM
    {
      get { return _isPCM; }
    }

    public bool IsDTS
    {
      get { return _isDTS; }
    }

    public bool IsOGG
    {
      get { return _isOGG; }
    }

    public bool IsAAC
    {
      get { return _isAAC; }
    }

    #endregion

    #region public misc properties

    public bool HasSubtitles
    {
      get { return _hasSubtitles; }
    }

    public int NumSubtitles
    {
        get { return _numSubtitles; }
    }

    public int Duration
    {
        get { return _duration; }
    }

    public bool HasChapters
    {
      get { return _hasChapters; }
    }

    #endregion

    #region public subtitle related properties

    public IEnumerable<SubtitleInfo> SubtitleInfo
    {
      get { return _subtitleInfo; }
    }

    #endregion
  }

  /// <summary>
  /// Information about Audio Streams
  /// </summary>
  public class AudioInfo
  {
      public int AudioStreamIndex { get; set; }
      public int AudioRate { get; set; }
      public int AudioChannels { get; set; }
      public string AudioCodecOriginal { get; set; }
      public string AudioFormatProfile { get; set; }

      /// <summary>
      /// 2-letter ISO 639-1 if exists, else 3-letter ISO 639-2, and with optional ISO 3166-1 country separated by a dash if available, e.g. en, en-us, zh-cn
      /// </summary>
      public string AudioLanguage { get; set; }
      internal string AudioTitle { get; set; }

      /// <summary>
      /// More info about Language (e.g. Director's Comment)
      /// </summary>
      internal string AudioLanguageMore { get; set; }

      /// <summary>
      /// Is the Audio Stream a commentary stream?
      /// </summary>
      public bool AudioIsCommentary
      {
        get
        {
          return AudioTitle.ToLower().Contains("comment") || AudioLanguageMore.ToLower().Contains("comment");
        }
      }

      /// <summary>
      /// Friendly name for <see cref="AudioChannels"/>
      /// </summary>
      public string AudioChannelsFriendly
      {
          get
          {
              return MediaInfoWrapper.GetAudioChannelsFriendly(AudioChannels);
          }
      }

      public string AudioCodec
      {
          get
          {
              if (IsTrueHD) return "TrueHD";
              if (IsDTSHD) return "DTSHD";
              if (IsAC3) return "AC3";
              if (IsMP3) return "MP3";
              if (IsMP2A) return "MP2A";
              if (IsDTS) return "DTS";
              if (IsOGG) return "VORBIS";
              if (IsAAC) return "AAC";
              if (IsWMA) return "WMA";
              if (IsPCM) return "PCM";
              return AudioCodecOriginal;
          }
      }

      public bool IsAC3 { get { return (System.Text.RegularExpressions.Regex.IsMatch(AudioCodecOriginal, "ac-?3")); } }
      public bool IsMP3 { get { return (AudioCodecOriginal.IndexOf("mpeg-1 audio layer 3") > -1) || (AudioCodecOriginal.IndexOf("mpeg-2 audio layer 3") > -1); } }
      public bool IsMP2A { get { return (AudioCodecOriginal.IndexOf("mpeg-1 audio layer 2") > -1); } }
      public bool IsDTS { get { return (AudioCodecOriginal.IndexOf("dts") > -1); } }
      public bool IsOGG { get { return (AudioCodecOriginal.IndexOf("ogg") > -1 || AudioCodecOriginal.IndexOf("vorbis") > -1); } }
      public bool IsAAC { get { return (AudioCodecOriginal.IndexOf("aac") > -1); } }
      public bool IsWMA { get { return (AudioCodecOriginal.IndexOf("wma") > -1); } } // e.g. wma3
      public bool IsPCM { get { return (AudioCodecOriginal.IndexOf("pcm") > -1); } }
      public bool IsTrueHD { get { return (AudioCodecOriginal.Contains("truehd") || AudioFormatProfile.Contains("truehd")); } }
      public bool IsDTSHD { get { return (AudioCodecOriginal.Contains("dts") && (AudioFormatProfile.Contains("hra") || AudioFormatProfile.Contains("ma"))); } }

  }

  /// <summary>
  /// Information about Subtitle Streams
  /// </summary>
  public class SubtitleInfo
  {
      /// <summary>
      /// 2-letter ISO 639-1 if exists, else 3-letter ISO 639-2, and with optional ISO 3166-1 country separated by a dash if available, e.g. en, en-us, zh-cn
      /// </summary>
      public string Language { get; set; }
      public bool Internal { get; set; }

      public SubtitleInfo()
      {
          Internal = true;
      }
  }

  
}

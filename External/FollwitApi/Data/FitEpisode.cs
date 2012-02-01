using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;

namespace Follwit.API.Data {
    /// <summary>
    /// Holds a collection of syncable episode Data
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class FitEpisode {

        /// <summary>
        /// Name of the source used to identify the episode via the SourceId property.
        /// example: follwit or tvdb
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Identity of the episode at the remote source
        /// example: follwit episode id, or thetvdb episode id
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// Follwit ID for the episode.
        /// </summary>
        public string FollwitId { get; set; }

        /// <summary>
        /// Indicates if the user has the episode in his collection (null to leave status untouched)
        /// </summary>
        public bool? InCollection { get; set; }

        /// <summary>
        /// Indicates if the user has watched the episode (null to leave status untouched)
        /// </summary>
        public bool? Watched { get; set; }

        /// <summary>
        /// The user provided rating (1-5), higher being better, 0 to remove the rating (null to leave status untouched)
        /// </summary>
        public int? Rating { get; set; }
    }
}

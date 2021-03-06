using System;
using System.Collections.Generic;
using System.Text;
using Cornerstone.Database;

namespace Cornerstone.GUI.Controls {
    interface IDBFieldBackedControl: IDBBackedControl {

        String DatabaseFieldName {
            get;
            set;
        }

        DBField DatabaseField {
            get;
        }

        DBField.DBDataType DBTypeOverride {
            get;
            set;
        } 
    }
}

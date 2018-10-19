using System.Collections.Generic;
using System.Linq;

namespace Sea.Utils
{
    /// <summary>
    /// 树形结构
    /// </summary>
    public class TreeModel
    {
        #region 属性 ID
        private string _ID = string.Empty;
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        #endregion

        #region 属性 PID
        private string _PID = string.Empty;
        public string PID
        {
            get
            {
                return _PID;
            }
            set
            {
                _PID = value;
            }
        }
        #endregion

        #region Children
        private List<TreeModel> _Children = null;
        public List<TreeModel> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = new List<TreeModel>();
                }
                return _Children;
            }
            set
            {
                _Children = value;
            }
        }
        #endregion
    }

    public static class TreeUtils
    {
        public static List<TreeModel> ConvertToTree(List<TreeModel> list)
        {
            if (list == null || list.Count == 0)
            {
                return new List<TreeModel>();
            }
            var dic = list.ToDictionary(k => k.ID, v => v);
            foreach (var item in dic.Values)
            {
                if (item != null && dic.ContainsKey(item.PID))
                {
                    dic[item.PID].Children.Add(item);
                }
            }
            return dic.Values.Where(v => string.IsNullOrEmpty(v.PID) || v.PID == "0").ToList();
        }
    }


}

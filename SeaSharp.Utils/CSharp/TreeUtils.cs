using System.Collections.Generic;
using System.Linq;

namespace SeaSharp.Utils
{
    /// <summary>
    /// 树形结构
    /// </summary>
    public class TreeModel
    {
        public string ID { get; set; } = string.Empty;
        public string PID { get; set; } = string.Empty;

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

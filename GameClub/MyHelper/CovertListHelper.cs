using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace GameClub.MyHelper
{
    public class CovertListHelper
    {
        public List<T> convertToList<T>(DataTable dt) where T : new()
        {
            //定义集合
            List<T> ts = new List<T>();
            //获得此模型的类型
            Type type = typeof(T);
            //定义一个临时的变量
            string tempName = "";
            //遍历datatable中所有数据行
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                //获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历所有属性
                foreach (PropertyInfo pi in propertys)
                {
                    //将此属性赋值给临时变量
                    tempName = pi.Name;
                    //检查datatable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {
                        //判断此属性是否有setter，这个啥意思呢，就是我们的实体层的{get;set;}如果我们的实体有了set方法，就说明可以赋值！
                        if (!pi.CanWrite) continue;
                        {
                            //取值  
                            object value = dr[tempName];
                            //if (value != DBNull.Value)
                            if(!string.IsNullOrEmpty(value.ToString()))
                            {
                                //根据实体类型转换值
                                //var obj = Convert.ChangeType(value, pi.PropertyType);
                                var obj = Convert.ChangeType(value, (Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType));
                                pi.SetValue(t, obj, null);
                            }
                                
                        }
                    }
                }
                //对象添加到泛型集合中
                ts.Add(t);
            }
            return ts;
        }
    }
}
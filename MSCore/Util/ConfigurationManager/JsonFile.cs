using MSCore.Util.Common;
using MSCore.Util.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using MSCore.Util.Logger;

namespace MSCore.Util.ConfigurationManager
{
    public class JsonFile
    {
        /// <summary>
        /// 把json数据保存到文件
        /// </summary>
        /// <param name="value">要保存的数据</param>
        /// <param name="filePath">json文件路径，例如：Data\\App.Robot.json</param>
        /// <param name="valueKeys">value在json文件中的json路径，可为null。例如：a.b.c ，若为null，则更新整个json文件</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void SetToFile(object value, string filePath, string valueKeys = null)
        {
            new JsonFile(filePath).SetByPath(value, valueKeys);
        }

        /// <summary>
        /// 若失败则返回空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">json文件路径，例如：Data\\App.Robot.json</param>
        /// <param name="valueKeys">value在json文件中的json路径，可为null。例如：a.b.c</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static T GetFromFile<T>(string filePath, string valueKeys = "")
        {
            try
            {
                return new JsonFile(filePath).Get<T>(valueKeys);
            }
            catch (Exception ex)
            {
               LoggerHelper.LogError(ex.ToString());
                return default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public JToken root { get; protected set; }
        /// <summary>
        /// json索引器
        /// <para>例如：jsonFile.Json["App"]</para>
        /// </summary>
        public JsonIndexer Json { get; protected set; }

        public string configPath { get; protected set; }

        /// <summary>
        /// 通过相对路径加载json文件
        /// </summary>
        /// <param name="path">ex：Data\\base.json</param>
        public JsonFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = Path.Combine(AppContext.BaseDirectory, path);

            configPath = path;

            RefreshJson();
        }

        /// <summary>
        /// 刷新Json文件，如果修改了Json文件需要刷新
        /// </summary>
        public virtual void RefreshJson()
        {
            root = null;
            Json = new JsonIndexer();
            try
            {
                string fileContent;
                if (!File.Exists(configPath) || string.IsNullOrEmpty(fileContent = File.ReadAllText(configPath)))
                {
                    root = new JObject();
                    Json = new JsonIndexer();
                    return;
                }
                root = JsonConvert.DeserializeObject(fileContent) as JToken;
                initIndexer();
            }
            catch { }

            if (root == null)
                root = new JObject();
            if (Json == null)
                Json = new JsonIndexer();
        }

        private void initIndexer()
        {
            var reader = root.CreateReader();
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Json[reader.Path] = root.SelectToken(reader.Path);
                }
            }
        }


        #region Set


        /// <summary>
        /// 会自动保存到原始json文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keys">value在Root中的json路径，可为null。例如：new []{"taskList",0,"name"}</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Set(object value, params object[] keys)
        {
            if (null == keys || keys.Length == 0)
            {
                root = value.ToJToken();
            }
            else
            {
                root.ValueSetByPath(value, keys);
            }
            SaveToFile();
        }


        /// <summary>
        /// 会自动保存到原始json文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path">value在Root中的json路径，可为null。例如："a.b.c"</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SetByPath(object value, string path)
        {
            Set(value, path?.Split('.'));
        }

        #endregion

        #region Get    


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual string GetStringByPath(string path)
        {
            var cur = root?.SelectToken(path);
            return cur.ConvertToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyStr">value在Root中的json路径，可为null。例如：a.b.c</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual T Get<T>(string keyStr)
        {
            var keys = keyStr?.Split('.');
            JToken cur = root;
            if (!string.IsNullOrEmpty(keyStr) && keys != null && keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    cur = cur?[key];
                }
            }
            return cur.Deserialize<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">value在Root中的json路径，可为null。例如："a.b.c"</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual T GetByPath<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
                return root.Deserialize<T>();

            return root.SelectToken(path).Deserialize<T>();
        }

        #endregion

        /// <summary>
        /// 保存到原始json文件
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual void SaveToFile()
        {
            if (string.IsNullOrEmpty(configPath)) return;

            try
            {
                string dir = Path.GetDirectoryName(configPath);
                if (!string.IsNullOrWhiteSpace(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(configPath, root.ToString());

            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
            }
        }
    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace TestScript
{
    public class ChartTest : MonoBehaviour
    {
        private void Awake()
        {
            string str = "{\n  \"pos\":[100,105]\n}";

            TestJsonType testJsonType = JsonConvert.DeserializeObject<TestJsonType>(str,new Vec2Conv());

            string con = JsonConvert.SerializeObject(testJsonType,new Vec2Conv());

            Debug.Log(con);

            Debug.Log(testJsonType.pos);
        }
    }

    public class TestJsonType
    {
        public Vector2 pos;
    }

    public class Vec2Conv : JsonConverter
    {
        /// <summary>
        /// 是否可以转化
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Vector2)) return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            JArray jArray = JArray.Load(reader);//加载Reader
            if (jArray.Count!=2)
            {
                throw new Exception("Error");
            }

            float x = jArray[0].ToObject<float>();
            float y = jArray[1].ToObject<float>();

            return new Vector2(x,y);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (Vector2)value;

            JArray jArray = new JArray(){v.x,v.y};
            
            jArray.WriteTo(writer:writer);
        }
    }
}
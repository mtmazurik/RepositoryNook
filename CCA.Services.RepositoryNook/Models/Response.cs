using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Models
{
    public class Response : IResponse
    {
        private Dictionary<string, string> _meta;
        private Object _data;

        public Response()           // ctor
        {
            _meta = new Dictionary<string, string>();
            _data = new JObject();
        }
        public Response(Object poco)    // plain old c# class serialized down the pipe by controller binding
        {
            _meta = new Dictionary<string, string>();
            _data = poco;
        }
        public Response(string key, string value)
        {
            _meta = new Dictionary<string, string>();
            _data = new JArray();
            _data = new JObject(new JProperty(key, value));
        }
        public Response(JObject jObject)
        {
            _meta = new Dictionary<string, string>();
            _data = jObject;
        }

        public Dictionary<string,string> Meta {  get { return _meta; } }

        public Object Data {
            get
            { return _data;  }
            set
            {
                _data = value;
            }
        }





        public void InsertStatusCodeIntoMeta(int statusCode)
        {
            this.Meta.Add("status", statusCode.ToString());
        }
        public void InsertExceptionIntoMeta(Exception exc)
        {
            this.Meta.Add("message", exc.Message);
            this.Meta.Add("details", exc.ToString());
            if (exc.InnerException != null)
            {
                this.Meta.Add("InnerException", exc.InnerException.ToString());
            }
        }
}
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Server.Common;
using WebServer.Server.HTTP.Contracts;

namespace WebServer.Server.HTTP
{
    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, ICollection<HttpHeader>> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, ICollection<HttpHeader>>();
        }

        public void Add(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));

            var headerKey = header.Key;

            if (!this.headers.ContainsKey(headerKey))
            {
                this.headers[header.Key] = new List<HttpHeader>();
            }

            this.headers[header.Key].Add(header);
        }

        public void Add(string key,string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.Add(new HttpHeader(key, value));
        }

        public bool ContainsKey(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            return this.headers.ContainsKey(key);
        }

        public ICollection<HttpHeader> Get(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));

            if (!this.headers.ContainsKey(key))
            {
                throw new InvalidOperationException($"The given key {key} is not present in the Header Collection");
            }

            return this.headers[key];
        }

        public IEnumerator<ICollection<HttpHeader>> GetEnumerator() => this.headers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
       
        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var header in this.headers)
            {
                var headerKey = header.Value;

                foreach (var headerValue in header.Value)
                {
                    result.AppendLine($"{headerKey}: {headerValue.Value}");
                }
            }
            return result.ToString();
        }

    }
}
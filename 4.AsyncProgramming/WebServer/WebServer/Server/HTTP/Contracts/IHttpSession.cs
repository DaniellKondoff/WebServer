﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Server.HTTP.Contracts
{
    public interface IHttpSession
    {
        string Id { get; }

        object Get(string key);

        T Get<T>(string key);

        void Add(string key, object value);

        void Clear();

        //bool IsAuthenticated();

        bool Contains(string key);
    }
}

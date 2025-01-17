/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Loxodon.Framework.Configurations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class ConfigurationExample : MonoBehaviour
    {
        void Start()
        {
            var conf        = CreateConfiguration();
            var appVersion  = conf.GetVersion("application.app.version");
            var dataVersion = conf.GetVersion("application.data.version");

            Debug.LogFormat("application.app.version:{0}",  appVersion);
            Debug.LogFormat("application.data.version:{0}", dataVersion);

            var groupName        = conf.GetString("application.config-group");
            var currentGroupConf = conf.Subset("application." + groupName);

            var upgradeUrl   = currentGroupConf.GetString("upgrade.url");
            var username     = currentGroupConf.GetString("username");
            var password     = currentGroupConf.GetString("password");
            var gatewayArray = currentGroupConf.GetArray<string>("gateway");

            Debug.LogFormat("upgrade.url:{0}", upgradeUrl);
            Debug.LogFormat("username:{0}",    username);
            Debug.LogFormat("password:{0}",    password);

            var i = 1;
            foreach (var gateway in gatewayArray) Debug.LogFormat("gateway {0}:{1}", i++, gateway);
        }

        IConfiguration CreateConfiguration()
        {
            var list = new List<IConfiguration>();

            //Load default configuration file
            var text = Resources.Load<TextAsset>("application.properties");
            list.Add(new PropertiesConfiguration(text.text));

            //Load configuration files based on platform information. Configuration files loaded later 
            //have a higher priority than configuration files loaded first.
            text = Resources.Load<TextAsset>(string.Format("application.{0}.properties", Application.platform.ToString().ToLower()));
            if (text != null)
                list.Add(new PropertiesConfiguration(text.text));

            if (list.Count == 1)
                return list[0];

            return new CompositeConfiguration(list);
        }
    }
}
﻿/*
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

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using Loxodon.Framework.Localizations;
using Loxodon.Log;
using System.Threading.Tasks;

namespace Loxodon.Framework.Examples
{
    /// <summary>
    /// Resources data provider.
    /// dir:
    /// root/default/
    /// 
    /// root/zh/
    /// root/zh-CN/
    /// root/zh-TW/
    /// root/zh-HK/
    /// 
    /// root/en/
    /// root/en-US/
    /// root/en-CA/
    /// root/en-AU/
    /// </summary>
    public class ResourcesDataProvider : IDataProvider
    {
        static readonly ILog log = LogManager.GetLogger(typeof(ResourcesDataProvider));

        string          root;
        IDocumentParser parser;

        public ResourcesDataProvider(string root, IDocumentParser parser)
        {
            if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");

            if (parser == null)
                throw new ArgumentNullException("parser");

            this.root   = root;
            this.parser = parser;
        }

        protected string GetDefaultPath() => GetPath("default");

        protected string GetPath(string dir)
        {
            var buf = new StringBuilder();
            buf.Append(root);
            if (!root.EndsWith("/"))
                buf.Append("/");
            buf.Append(dir);
            return buf.ToString();
        }

        public virtual Task<Dictionary<string, object>> Load(CultureInfo cultureInfo)
        {
            var dict = new Dictionary<string, object>();
            try
            {
                var defaultTexts      = Resources.LoadAll<TextAsset>(GetDefaultPath());                                                                                 //eg:default
                var twoLetterISOTexts = Resources.LoadAll<TextAsset>(GetPath(cultureInfo.TwoLetterISOLanguageName));                                                    //eg:zh  en
                var texts             = cultureInfo.Name.Equals(cultureInfo.TwoLetterISOLanguageName) ? null : Resources.LoadAll<TextAsset>(GetPath(cultureInfo.Name)); //eg:zh-CN  en-US

                FillData(dict, defaultTexts,      cultureInfo);
                FillData(dict, twoLetterISOTexts, cultureInfo);
                FillData(dict, texts,             cultureInfo);
                return Task.FromResult(dict);
            }
            catch (Exception e)
            {
                return Task.FromException<Dictionary<string, object>>(e);
            }
        }

        void FillData(Dictionary<string, object> dict, TextAsset[] texts, CultureInfo cultureInfo)
        {
            try
            {
                if (texts == null || texts.Length <= 0)
                    return;

                foreach (var text in texts)
                    try
                    {
                        using (var stream = new MemoryStream(text.bytes))
                        {
                            var data                              = parser.Parse(stream, cultureInfo);
                            foreach (var kv in data) dict[kv.Key] = kv.Value;
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("An error occurred when loading localized data from \"{0}\".Error:{1}", text.name, e);
                    }
            }
            catch (Exception)
            {
            }
        }
    }
}
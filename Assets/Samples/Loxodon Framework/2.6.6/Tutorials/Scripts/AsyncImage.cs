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

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Tutorials
{
    public class AsyncImage : Image
    {
        string                  spriteName;
        Material                originMaterial;
        CancellationTokenSource source;
        public Sprite           loadingSprite;
        public Material         loadingMaterial;

        public string SpriteName
        {
            get => spriteName;
            set
            {
                if (string.Equals(spriteName, value))
                    return;

                spriteName = value;
                OnSpriteNameChanged(spriteName);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            originMaterial = material;
        }

        protected async void OnSpriteNameChanged(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                material = originMaterial;
                sprite   = null;
                return;
            }

            if (source != null)
                source.Cancel();

            source = new();
            var token = source.Token;
            try
            {
                this.sprite = loadingSprite;
                material    = loadingMaterial;
                var sprite = await LoadSprite(spriteName);
                if (!token.IsCancellationRequested)
                {
                    material    = originMaterial;
                    this.sprite = sprite;
                    source      = null;
                }
            }
            catch
            {
                if (!token.IsCancellationRequested)
                {
                    material = originMaterial;
                    sprite   = null;
                    source   = null;
                }
            }
        }

        protected async Task<Sprite> LoadSprite(string spriteName) => (Sprite)await Resources.LoadAsync<Sprite>(spriteName);
    }
}
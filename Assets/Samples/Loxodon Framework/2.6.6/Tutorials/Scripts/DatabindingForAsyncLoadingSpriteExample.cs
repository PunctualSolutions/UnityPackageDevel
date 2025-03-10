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

using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class SpriteViewModel : ViewModelBase
    {
        string spriteName = "EquipImages_1";

        public string SpriteName
        {
            get => spriteName;
            set => Set(ref spriteName, value);
        }

        public void ChangeSpriteName()
        {
            SpriteName = string.Format("EquipImages_{0}", Random.Range(1, 30));
        }
    }

    public class DatabindingForAsyncLoadingSpriteExample : MonoBehaviour
    {
        public Button changeSpriteButton;

        public AsyncSpriteLoader spriteLoader;

        void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
        }

        void Start()
        {
            var viewModel = new SpriteViewModel();

            var bindingContext = this.BindingContext();
            bindingContext.DataContext = viewModel;

            var bindingSet = this.CreateBindingSet<DatabindingForAsyncLoadingSpriteExample, SpriteViewModel>();
            bindingSet.Bind(spriteLoader).For(v => v.SpriteName).To(vm => vm.SpriteName).OneWay();

            bindingSet.Bind(changeSpriteButton).For(v => v.onClick).To(vm => vm.ChangeSpriteName);

            bindingSet.Build();
        }
    }
}
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
using Loxodon.Framework.Binding.Converters;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views.InteractionActions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class ListViewDatabindingExample : MonoBehaviour
    {
        ListViewViewModel viewModel;

        public Button addButton;

        public Button removeButton;

        public Button clearButton;

        public Button changeIconButton;

        public Button changeItems;

        public ListView listView;

        public ListItemDetailView detailView;

        public ListItemEditView editView;

        AsyncViewInteractionAction editViewInteractionAction;

        void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            var sprites = new Dictionary<string, Sprite>();
            foreach (var sprite in Resources.LoadAll<Sprite>("EquipTextures"))
                if (sprite != null)
                    sprites.Add(sprite.name, sprite);
            var converterRegistry = context.GetContainer().Resolve<IConverterRegistry>();
            converterRegistry.Register("spriteConverter", new SpriteConverter(sprites));
        }

        void OnDestroy()
        {
            var context           = Context.GetApplicationContext();
            var converterRegistry = context.GetContainer().Resolve<IConverterRegistry>();
            converterRegistry.Unregister("spriteConverter");
        }

        void Start()
        {
            editViewInteractionAction = new(editView);
            viewModel                 = new();
            var bindingContext = this.BindingContext();
            bindingContext.DataContext = viewModel;

            var bindingSet = this.CreateBindingSet<ListViewDatabindingExample, ListViewViewModel>();
            bindingSet.Bind(listView).For(v => v.Items).To(vm => vm.Items).OneWay();
            bindingSet.Bind(detailView).For(v => v.Item).To(vm => vm.SelectedItem);
            bindingSet.Bind().For(v => v.editViewInteractionAction).To(vm => vm.ItemEditRequest);

            bindingSet.Bind(addButton).For(v => v.onClick).To(vm => vm.AddItem);
            bindingSet.Bind(removeButton).For(v => v.onClick).To(vm => vm.RemoveItem);
            bindingSet.Bind(clearButton).For(v => v.onClick).To(vm => vm.ClearItem);
            bindingSet.Bind(changeIconButton).For(v => v.onClick).To(vm => vm.ChangeItemIcon);
            bindingSet.Bind(changeItems).For(v => v.onClick).To(vm => vm.ChangeItems);

            bindingSet.Build();

            viewModel.SelectItem(0);
        }
    }
}
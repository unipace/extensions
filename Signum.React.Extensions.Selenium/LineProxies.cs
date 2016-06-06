﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Signum.Engine.Basics;
using Signum.Entities;
using Signum.Entities.Reflection;
using Signum.Utilities;
using Signum.Utilities.Reflection;
using Signum.React.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Signum.React.Selenium
{

    public class BaseLineProxy
    {
        public IWebElement Element { get; private set; }

        public PropertyRoute Route { get; private set; }

        public BaseLineProxy(IWebElement element, PropertyRoute route)
        {
            this.Element = element;
            this.Route = route;
        }

        protected static string ToVisible(bool visible)
        {
            return visible ? "visible" : "not visible";
        }
    }

    public class ValueLineProxy : BaseLineProxy
    {
        public ValueLineProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public string StringValue
        {
            get
            {
                IWebElement checkBox = this.Element.FindElement(By.CssSelector("input[type=checkbox]"));
                if (checkBox != null)
                    return checkBox.Selected.ToString();

                IWebElement namedElement = this.Element.TryFindElement(By.CssSelector("input[type=text]"));
                if(namedElement != null)
                    return namedElement.GetAttribute("value");

                IWebElement date = this.Element.TryFindElement(By.Name("Date"));
                IWebElement time = this.Element.TryFindElement(By.Name("Time"));

                if (date != null && time != null)
                    return date.GetAttribute("value") + " " + time.GetAttribute("value");

                if (checkBox != null)
                    return checkBox.Text;
            
                throw new InvalidOperationException("Element {0} not found".FormatWith(this.Route.PropertyString()));
            }

            set
            {

                IWebElement checkBox = this.Element.TryFindElement(By.CssSelector("input[type=checkbox]"));
                if (checkBox != null)
                {
                    checkBox.SetChecked(bool.Parse(value));
                }

                IWebElement byName = this.Element.TryFindElement(By.CssSelector("input[type=text]"));
                if (byName != null)
                {
                    if (byName.TagName == "select")
                        byName.SelectElement().SelectByValue(value);
                    else
                        byName.SafeSendKeys(value);
                    return;
                }
                else
                    throw new InvalidOperationException("Element {0} not found".FormatWith(this.Route));
            }
        }

        public WebElementLocator MainElement
        {
            get { return this.Element.WithLocator(By.CssSelector("input")); }
        }

        public object Value
        {
            get { return GetValue(Route.Type); }
            set { SetValue(value, Reflector.FormatString(Route)); }
        }

        public object GetValue(Type type)
        {
            return ReflectionTools.Parse(StringValue, type);
        }

        public T GetValue<T>()
        {
            return ReflectionTools.Parse<T>(StringValue); 
        }

        public void SetValue(object value, string format = null)
        {
            StringValue = value == null ? null :
                    value is IFormattable ? ((IFormattable)value).ToString(format, null) :
                    value.ToString();
        }
    }

    public abstract class EntityBaseProxy : BaseLineProxy
    {
        public EntityBaseProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public virtual PropertyRoute ItemRoute => this.Route;

        public WebElementLocator CreateButton
        {
            get { return this.Element.WithLocator(By.CssSelector("a.sf-create")); }
        }

        

        protected void CreateEmbedded<T>()
        {
            WaitChanges(() =>
            {
                var imp = this.ItemRoute.TryGetImplementations();
                if (imp != null && imp.Value.Types.Count() != 1)
                {
                    var popup = this.CreateButton.Find().CaptureOnClick();
                    ChooseType(typeof(T), popup);
                }
                else
                {
                    this.CreateButton.Find().Click();
                }
            }, "create clicked");
        }

        public PopupControl<T> CreatePopup<T>() where T : ModifiableEntity
        {
         
            string changes = GetChanges();

            var popup = this.CreateButton.Find().CaptureOnClick();

            popup = ChooseTypeCapture(typeof(T), popup);

            return new PopupControl<T>(popup, this.ItemRoute)
            {
                Disposing = okPressed => { WaitNewChanges(changes, "create dialog closed"); }
            };
        }

        public WebElementLocator ViewButton
        {
            get { return this.Element.WithLocator(By.CssSelector("a.sf-view")); }
        }
        
        protected PopupControl<T> ViewInternal<T>() where T : ModifiableEntity
        {
            var newElement = this.ViewButton.Find().CaptureOnClick();
            string changes = GetChanges();
            
            return new PopupControl<T>(newElement, this.ItemRoute)
            {
                Disposing = okPressed => WaitNewChanges(changes, "create dialog closed")
            };
        }

        public WebElementLocator FindButton
        {
            get { return this.Element.WithLocator(By.CssSelector("a.sf-find")); }
        }

        public WebElementLocator RemoveButton
        {
            get { return this.Element.WithLocator(By.CssSelector("a.sf-remove")); }
        }

        public void Remove()
        {
            WaitChanges(() => this.RemoveButton.Find().Click(), "removing");
        }
      
        public SearchPopupProxy Find(Type selectType = null)
        {
            string changes = GetChanges();
            var popup = FindButton.Find().CaptureOnClick();

            popup = ChooseTypeCapture(selectType, popup);

            return new SearchPopupProxy(popup)
            {
                Disposing = okPressed => { WaitNewChanges(changes, "create dialog closed"); }
            };
        }

        private void ChooseType(Type selectType, IWebElement element)
        {
            if (!SelectorModal.IsSelector(element))
                return;

            if (selectType == null)
                throw new InvalidOperationException("No type to choose from selected");

            SelectorModal.Select(this.Element, TypeLogic.GetCleanName(selectType));
        }

        private IWebElement ChooseTypeCapture(Type selectType, IWebElement element)
        {
            if (!SelectorModal.IsSelector(element))
                return element;

            if (selectType == null)
                throw new InvalidOperationException("No type to choose from selected");

            var newElement = element.GetDriver().CapturePopup(() =>
                SelectorModal.Select(this.Element, TypeLogic.GetCleanName(selectType)));

            return newElement;
        }

        public void WaitChanges(Action action, string actionDescription)
        {
            var changes = GetChanges();

            action();

            WaitNewChanges(changes, actionDescription);
        }

        public void WaitNewChanges(string changes, string actionDescription)
        {
            Element.GetDriver().Wait(() => GetChanges() != changes, () => "Waiting for changes after {0} in {1}".FormatWith(actionDescription, this.Route.ToString()));
        }

        public string GetChanges()
        {
            return this.Element.GetAttribute("data-changes");
        }

        protected EntityInfoProxy EntityInfoInternal(int? index)
        {
            var element = index == null ? Element.FindElement(By.CssSelector("[data-entity]")) :
            this.Element.FindElements(By.CssSelector("[data-entity]")).ElementAt(index.Value);

            return EntityInfoProxy.Parse(element.GetAttribute("data-entity"));
        }

        protected void AutoCompleteAndSelect(IWebElement autoCompleteElement, Lite<IEntity> lite)
        {
            WaitChanges(() =>
            {
                autoCompleteElement.FindElement(By.CssSelector("input")).SafeSendKeys(lite.Id.ToString());
                //Selenium.FireEvent(autoCompleteLocator, "keyup");

                var listLocator = By.CssSelector("ul.typeahead.dropdown-menu");

                autoCompleteElement.WaitElementVisible(listLocator);
                IWebElement itemElement = autoCompleteElement.FindElement(By.CssSelector("[data-entity-key='{0}']".FormatWith(lite.Key())));

                itemElement.Click();

            }, "autocomplete selection");
        }
    }

    public class EntityInfoProxy
    {
        public bool IsNew { get; set; }
        public string TypeName { get; set; }

        public Type EntityType;
        public PrimaryKey? IdOrNull { get; set; }
        

        public Lite<Entity> ToLite(string toString = null) => Lite.Create(this.EntityType, this.IdOrNull.Value, null);

        internal static EntityInfoProxy Parse(string dataEntity)
        {
            if (dataEntity == "null")
                return null;

            var parts = dataEntity.Split(';');

            var typeName = parts[0];
            var id = parts[1];
            var isNew = parts[2];

            var type = TypeLogic.TryGetType(typeName);

            return new EntityInfoProxy
            {
                TypeName = typeName,
                EntityType = type,
                IdOrNull = id.HasText() ? PrimaryKey.Parse(id, type) : (PrimaryKey?)null,
                IsNew = isNew.HasText() && bool.Parse(isNew)
            };
        }
    }

    public class EntityLineProxy : EntityBaseProxy
    {
        public EntityLineProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public Lite<IEntity> LiteValue
        {
            get { return EntityInfo()?.ToLite(); }
            set
            {
                if (this.EntityInfo() != null)
                    this.Remove();

                if (value != null)
                {
                    if (AutoCompleteElement.IsVisible())
                        AutoComplete(value);
                    else if (FindButton != null)
                        this.Find().SelectLite(value);
                    else
                        throw new NotImplementedException("AutoComplete");
                }
            }
        }

        public WebElementLocator AutoCompleteElement
        {
            get { return this.Element.WithLocator(By.CssSelector(".sf-typeahead")); }
        }

        public void AutoComplete(Lite<IEntity> lite)
        {
            base.AutoCompleteAndSelect(AutoCompleteElement.Find(), lite);
        }

        public PopupControl<T> View<T>() where T : ModifiableEntity
        {
            return base.ViewInternal<T>();
        }

        public EntityInfoProxy EntityInfo()
        {
            return EntityInfoInternal(null);
        }
    }

    public class EntityComboProxy : EntityBaseProxy
    {
        public EntityComboProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public SelectElement ComboElement
        {
            get { return this.Element.FindElement(By.CssSelector("select")).SelectElement(); }
        }

        public Lite<IEntity> LiteValue
        {
            get
            {
                var text =  this.ComboElement.AllSelectedOptions.SingleOrDefaultEx()?.Text;

                return EntityInfo().ToLite(text);
            }
            set
            {
                this.ComboElement.SelectByValue(value == null ? "" : value.Key());
            }
        }

        public List<Lite<Entity>> Options()
        {
            return this.ComboElement.Options
                .Select(o => Lite.Parse(o.GetAttribute("value"))?.Do(l => l.SetToString(o.Text)))
                .ToList();
        }

        public PopupControl<T> View<T>() where T : ModifiableEntity
        {
            return base.ViewInternal<T>();
        }

        public void SelectLabel(string label)
        {
            WaitChanges(() =>
                this.ComboElement.SelectByText(label),
                "ComboBox selected");
        }

        public void SelectIndex(int index)
        {
            WaitChanges(() =>
                this.ComboElement.SelectByIndex(index + 1),
                "ComboBox selected");
        }

        public EntityInfoProxy EntityInfo()
        {
            return EntityInfoInternal(null);
        }
    }

    public class EntityDetailProxy : EntityBaseProxy
    {
        public EntityDetailProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public Lite<IEntity> Lite
        {
            get { return EntityInfo()?.ToLite(); }
            set
            {
                if (this.EntityInfo() != null)
                    this.Remove();

                if (this.FindButton.IsVisible())
                    this.Find().SelectLite(value);

                throw new NotImplementedException("AutoComplete");
            }
        }

        public LineContainer<T> Details<T>() where T : ModifiableEntity
        {
            return new LineContainer<T>(this.Element.FindElement(By.CssSelector("div[data-propertypath]")), Route);
        }

        public EntityInfoProxy EntityInfo()
        {
            return EntityInfoInternal(null);
        }

        public ILineContainer<T> GetOrCreateDetailControl<T>() where T : ModifiableEntity
        {
            if (this.EntityInfo() !=null)
                return this.Details<T>();

            CreateEmbedded<T>();

            return this.Details<T>();
        }
    }

    public class EntityListProxy : EntityBaseProxy
    {
        public override PropertyRoute ItemRoute => base.ItemRoute.Add("Item");

        public EntityListProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public WebElementLocator OptionElement(int index)
        {
            return this.ListElement.Find().WithLocator(By.CssSelector("option:nth-child({0})".FormatWith(index)));
        }

        public WebElementLocator ListElement
        {
            get { return this.Element.WithLocator(By.CssSelector("select.form-control")); }
        }

        public void Select(int index)
        {
            this.OptionElement(index).Find().Click();
        }

        public PopupControl<T> View<T>(int index) where T : ModifiableEntity
        {
            Select(index);

            return base.ViewInternal<T>();
        }
        
        public int ItemsCount()
        {
            return this.ListElement.Find().FindElements(By.CssSelector("option")).Count;
        }

        public EntityInfoProxy EntityInfo(int index)
        {
            return EntityInfoInternal(index);
        }
        
        public void DoubleClick(int index)
        {
            Select(index);
            OptionElement(index).Find().DoubleClick();
        }
    }
    

    public class EntityRepeaterProxy : EntityBaseProxy
    {
        public override PropertyRoute ItemRoute => base.ItemRoute.Add("Item");

        public EntityRepeaterProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public virtual WebElementLocator ItemsContainerElement
        {
            get { return this.Element.WithLocator(By.CssSelector("sf-repater-elements")); }
        }

        public virtual WebElementLocator ItemElement(int index)
        {
            return this.ItemsContainerElement.CombineCss(" > fieldset.sf-repeater-element:nth-child({0})".FormatWith(index));
        }

        public void WaitItemLoaded(int index)
        {
            ItemElement(index).WaitPresent();
        }

        public virtual void MoveUp(int index)
        {
            ItemElement(index).CombineCss(" a.move-up").Find().Click();
        }

        public virtual void MoveDown(int index)
        {
            ItemElement(index).CombineCss(" a.move-down").Find().Click();
        }

        public virtual int ItemsCount()
        {
            return this.ItemsContainerElement.CombineCss(" > fieldset.sf-repeater-element§").FindElements().Count;
        }
        
        public LineContainer<T> Details<T>(int index) where T : ModifiableEntity
        {
            return new LineContainer<T>(ItemElement(index).Find(), this.Route);
        }

        public IWebElement RemoveElementIndex(int index)
        {
            return ItemElement(index).CombineCss(" a.remove").Find();
        }

        public void Remove(int index)
        {
            this.RemoveElementIndex(index).Click();
        }

        public EntityInfoProxy EntityInfo(int index)
        {
            return EntityInfoInternal(index);
        }

        public LineContainer<T> CreateElement<T>() where T : ModifiableEntity
        {
            var count = this.ItemsCount();

            CreateEmbedded<T>();

            return this.Details<T>(count + 1);
        }

        public LineContainer<T> LastDetails<T>() where T : ModifiableEntity
        {
            return this.Details<T>(this.ItemsCount() + 1);
        }
    }

    public class EntityTabRepeaterProxy : EntityRepeaterProxy
    {
        public EntityTabRepeaterProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }
    }

    public class EntityStripProxy : EntityBaseProxy
    {
        public EntityStripProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public WebElementLocator ItemsContainerElement
        {
            get { return this.Element.WithLocator(By.CssSelector("ul.sf-strip")); }
        }

        public WebElementLocator StripItemSelector(int index)
        {
            return this.ItemsContainerElement.CombineCss(" > li.sf-strip-element:nth-child({0})".FormatWith(index));
        }

        public int ItemsCount()
        {
            return this.ItemsContainerElement.CombineCss(" > li.sf-strip-element").FindElements().Count;
        }

        public WebElementLocator ViewElementIndex(int index)
        {
            return StripItemSelector(index).CombineCss(" > a.sf-entitStrip-link");
        }

        public WebElementLocator RemoveElementIndex(int index)
        {
            return StripItemSelector(index).CombineCss(" > a.sf-remove");
        }

        public void Remove(int index)
        {
            RemoveElementIndex(index).Find().Click();
        }

        public EntityInfoProxy EntityInfo(int index)
        {
            return EntityInfoInternal(index);
        }

        public WebElementLocator AutoCompleteElement
        {
            get { return this.Element.WithLocator(By.CssSelector(".sf-typeahead")); }
        }

        public void AutoComplete(Lite<IEntity> lite)
        {
            base.AutoCompleteAndSelect(AutoCompleteElement.Find(), lite);
        }

        public PopupControl<T> View<T>(int index) where T : ModifiableEntity
        {
            var changes = this.GetChanges();
            var popup = ViewElementIndex(index).Find().CaptureOnClick();

            return new PopupControl<T>(popup, this.ItemRoute)
            {
                Disposing = okPressed => WaitNewChanges(changes, "create dialog closed")
            };
        }
    }

    public class EntityListCheckBoxProxy : EntityBaseProxy
    {
        public EntityListCheckBoxProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public WebElementLocator CheckBoxElement(Lite<Entity> lite)
        {
            return this.Element.WithLocator(By.CssSelector("input[name='{0}']".FormatWith(lite.Key())));
        }

        public List<Lite<Entity>> GetDataElements()
        {
            return this.Element.WithLocator(By.CssSelector("label.sf-checkbox-element")).FindElements().Select(e =>
            {
                var lite = Lite.Parse(e.FindElement(By.CssSelector("input[type=checkbox]")).GetAttribute("name"));
                lite.SetToString(e.FindElement(By.CssSelector("div.sf-entitStrip-link")).Text);
                return lite;
            }).ToList();
        }

        public void SetChecked(Lite<Entity> lite, bool isChecked)
        {
            CheckBoxElement(lite).Find().SetChecked(isChecked);
        }
    }


    public class FileLineProxy : BaseLineProxy
    {
        public FileLineProxy(IWebElement element, PropertyRoute route)
            : base(element, route)
        {
        }

        public void SetPath(string path)
        {
            FileElement.Find().SendKeys(path);
            FileElement.WaitNoPresent();
        }

        private WebElementLocator FileElement
        {
            get { return this.Element.WithLocator(By.CssSelector("input[type=file]")); }
        }
    }
}
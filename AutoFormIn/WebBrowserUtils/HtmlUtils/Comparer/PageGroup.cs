using Assistant.DataProviders;
using AssistantUpdater;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Detectors;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    class PageGroup
    {
        private string _pageName;
        private Office.Excel.ForwardExcelReader oldWorkbook, newWorkbook;
        private bool _hasChanged;
        private RuleCompareNode parent;
        private List<RuleItem> _oldPageitems, _newPageItems;

        public RuleCompareNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public List<RuleItem> OldPageItems
        {
            get { return _oldPageitems; }
        }

        public List<RuleItem> NewPageItems
        {
            get { return _newPageItems; }
        }

        public bool HasChanged
        {
            get { return _hasChanged; }
        }

        public string PageName
        {
            get { return _pageName; }
        }

        public PageGroup(string pageName, Office.Excel.ForwardExcelReader oldWorkbook, Office.Excel.ForwardExcelReader newWorkbook)
        {
            this.oldWorkbook = oldWorkbook;
            this.newWorkbook = newWorkbook;
            _oldPageitems = new List<RuleItem>();
            _newPageItems = new List<RuleItem>();
            _pageName = pageName;
        }

        public void ReadRules()
        {
            _oldPageitems.Clear();
            _newPageItems.Clear();

            Office.Excel.ForwardReadWorksheet sheet = oldWorkbook == null ? null
                : oldWorkbook.Activate(_pageName) as Office.Excel.ForwardReadWorksheet;
            if (sheet != null)
            {
                sheet.Reset();
                GetParameters(sheet, _oldPageitems);
            }
            sheet = newWorkbook.Activate(_pageName) as Office.Excel.ForwardReadWorksheet;
            if (sheet != null)
            {
                sheet.Reset();
                GetParameters(sheet, _newPageItems);
            }
        }

        public void Compare()
        {
            List<KeyValuePair<string, List<RuleItem>>> oldItems = new List<KeyValuePair<string,List<RuleItem>>>();
            List<KeyValuePair<string, List<RuleItem>>> newItems = new List<KeyValuePair<string, List<RuleItem>>>();
            List<RuleItem> current = null;
            foreach (var item in _oldPageitems)
            {
                if (item.Type == Matcher.TYPE_FORM)
                {
                    current = new List<RuleItem>();
                    oldItems.Add(new KeyValuePair<string, List<RuleItem>>(item.Name, current));
                }
                if (current != null)
                {
                    current.Add(item);
                }
            }
            current = null;
            foreach (var item in _newPageItems)
            {
                if (item.Type == Matcher.TYPE_FORM)
                {
                    current = new List<RuleItem>();
                    newItems.Add(new KeyValuePair<string, List<RuleItem>>(item.Name, current));
                }
                if (current != null)
                {
                    current.Add(item);
                }
            }
            int index1 = 0;
            List<RuleItem> empty = new List<RuleItem>();
            for (; index1 < oldItems.Count; index1++)
            {
                if (index1 < newItems.Count)
                    CompareForm(oldItems[index1].Value, newItems[index1].Value);
                else
                    CompareForm(oldItems[index1].Value, empty);
            }
            for (; index1 < newItems.Count; index1++)
            {
                CompareForm(empty, newItems[index1].Value);
            }
        }

        private void CompareForm(List<RuleItem> form1, List<RuleItem> form2)
        {
            foreach (var item in form1)
            {
                if (form2.Contains(item) == false)
                {
                    if (parent != null)
                        parent.HasChange = true;
                    item.Status = RuleItemStatus.Removed;
                }
            }
            foreach (var item in form2)
            {
                if (form1.Contains(item) == false)
                {
                    if (parent != null)
                        parent.HasChange = true;
                    item.Status = RuleItemStatus.Added;
                }
            }
        }

        private void GetParameters(Office.Excel.ForwardReadWorksheet sheet, List<RuleItem> items)
        {
            Hashtable columnHeader = new Hashtable();
            if (sheet == null)
                return;
            object content = null;
            if (sheet.ReadNextRow())
            {
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    columnHeader.Add(sheet.CurrentCell.ColumnIndex, content == null ? "" : content.ToString());
                }
            }
            RuleItem ruleItem;
            while (sheet.ReadNextRow())
            {
                ruleItem = new RuleItem();
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    string value = content == null ? "" : content.ToString();
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "元素id":
                        ruleItem.Id = value;
                        break;
                    case "元素name":
                        ruleItem.Name = value;
                        break;
                    case "元素value":
                        ruleItem.Value = value;
                        break;
                    case "类别":
                        ruleItem.Type = value == null ? "" : value.ToUpper();
                        break;
                    case "元素onclick":
                        ruleItem.OnClick = value;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(ruleItem.Type))
                    continue;
                items.Add(ruleItem);
            }
        }
    }
}

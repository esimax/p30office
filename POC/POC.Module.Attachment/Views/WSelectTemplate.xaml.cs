using System;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Core;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.AC;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.Attachment.Views
{
    public partial class WSelectTemplate : DXWindow
    {
        private IDatabase ADatabase { get; set; }

        public DBACFactorReportTemplate SelectedItem { get; set; }
        public DBACFactor Factor { get; set; }

        public WSelectTemplate()
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            Loaded += (s1, e1) =>
            {
                using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                {
                    var list = new XPQuery<DBACFactorReportTemplate>(uow).OrderBy(n => n.Title).ToList();
                    gcMain.ItemsSource = list;
                    if (list.Any())
                        btOk.IsEnabled = true;
                }

                gcMain.SelectedItemChanged += (s, e) =>
                {
                    btOk.IsEnabled = gcMain.SelectedItem != null;
                    SelectedItem = gcMain.SelectedItem as DBACFactorReportTemplate;
                };

                gcMain.MouseDoubleClick +=
                (s, e) =>
                {
                    if (!btOk.IsEnabled)
                        return;
                    var i = tvMain.GetRowHandleByMouseEventArgs(e);
                    if (i < 0)
                        return;
                    SelectedItem = gcMain.SelectedItem as DBACFactorReportTemplate;
                    ShowHtml();
                    e.Handled = true;
                };

                btOk.Click += (s, e) =>
                {
                    SelectedItem = gcMain.SelectedItem as DBACFactorReportTemplate;
                    ShowHtml();
                };
            };
        }

        private void ShowHtml()
        {
            var pattern = @"({){2}(.*)(}){2}";
            var html = "";
            using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
            {
                html = new XPQuery<DBACFactorReportTemplate>(uow).First(n => n.Oid == SelectedItem.Oid).FileData;
            }

            var regex = new Regex(pattern);
            var match = regex.Match(html);
            while (match.Success)
            {
                ProcessJsonData(ref html, match.Index, match.Length, match.Value);
                match = regex.Match(html);
            }

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = System.IO.Path.Combine(path, "Factor");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            var fn = System.IO.Path.Combine(path, $"XOffice-Factor-{DateTime.Now.Ticks}.html");
            System.IO.File.AppendAllText(fn, html);
            System.Diagnostics.Process.Start(fn);
            DialogResult = true;
            Close();
        }

        private void ProcessJsonData(ref string html, int index, int length, string value)
        {
            try
            {
                var jsonText = value.Replace("{{", "{");
                jsonText = jsonText.Replace("}}", "}");
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText);
                var valueType = f(json.Type.ToString());

                if (valueType == f("فاکتور"))
                {
                    var name = f(json.Name.ToString());
                    if (name == f("کد"))
                        SetData(ref html, index, length, value, Factor.Code.ToString());
                    else if (name == f("تاریخ"))
                        SetData(ref html, index, length, value, HelperPersianCalendar.ToString(Factor.Date, "yyyy/MM/dd"));
                    else if (name == f("پیش فاکتور"))
                        SetData(ref html, index, length, value, f(Factor.IsPreFactor ? "پیش فاکتور" : "فاکتور"));
                    else if (name == f("پرونده:عنوان"))
                        SetData(ref html, index, length, value, f(Factor.Contact.Title));
                    else if (name == f("پرونده:کد"))
                        SetData(ref html, index, length, value, f(Factor.Contact.Code.ToString()));
                    else if (name == f("عنوان"))
                        SetData(ref html, index, length, value, f(Factor.Title));
                    else if (name == f("عنوان چاپی"))
                        SetData(ref html, index, length, value, f(Factor.TitleOfTitle));
                    else if (name == f("ارزش افزوده"))
                        SetData(ref html, index, length, value, f((Factor.PercentIncrease * 100).ToString("N0")));
                    else if (name == f("تخفیف:درصد"))
                        SetData(ref html, index, length, value, f((Factor.PercentDiscount * 100).ToString("N0")));
                    else if (name == f("تخفیف:ریال"))
                        SetData(ref html, index, length, value, f(Factor.AmountDiscount.ToString("N0")));
                    else if (name == f("نکته"))
                        SetData(ref html, index, length, value, f(Factor.Note));
                    else if (name == f("جمع"))
                        SetData(ref html, index, length, value, f(Factor.FactoreItems.Sum(n => (double)n.Price * n.Count).ToString("N0")));
                    else if (name == f("جمع با تخفیف"))
                    {
                        var sum = Factor.FactoreItems.Sum(n => (double)n.Price * n.Count);
                        sum = sum - (double)Factor.AmountDiscount;
                        sum = sum - (sum * (double)Factor.PercentDiscount);
                        sum = sum + (sum * (double)Factor.PercentIncrease);
                        SetData(ref html, index, length, value, f(sum.ToString("N0")));
                    }
                    else if (name == f("جمع با ارزش افزوده"))
                    {
                        var sum = Factor.FactoreItems.Sum(n => (double)n.Price * n.Count);
                        sum = sum - (double)Factor.AmountDiscount;
                        sum = sum - (sum * (double)Factor.PercentDiscount);
                        sum = sum + (sum * (double)Factor.PercentIncrease);
                        SetData(ref html, index, length, value, f(sum.ToString("N0")));
                    }
                    else if (name == f("اطلاعات تکمیلی"))
                    {
                        var variableName = f(json.VariableName.ToString());
                        var templateDataSplit = Factor.TemplateValues.Split(new[] { '|' },
                            StringSplitOptions.RemoveEmptyEntries);
                        var templateValue = templateDataSplit.FirstOrDefault(n => n.StartsWith(variableName + ";"));
                        if (!string.IsNullOrEmpty(templateValue))
                        {
                            templateValue =
                                templateValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? "";
                            if (templateValue.StartsWith(":"))
                                templateValue = templateValue.TrimStart(':');
                            SetData(ref html, index, length, value, f(templateValue));
                        }
                        else
                        {
                            SetData(ref html, index, length, value, "");
                        }
                    }
                    else
                    {
                        SetData(ref html, index, length, value, "");
                    }
                }
                if (valueType == f("اقلام فاکتور"))
                {
                    var id = json.Id.ToString();
                    var pattern = @"(<script id='" + id + @"' type='x-tmpl-xoffice'>)([.|\n|\W|\w]*)(</script>)";
                    var regex = new Regex(pattern);
                    var match = regex.Match(html);
                    var allList = "";
                    if (match.Success)
                    {
                        var factorItemTemplate = match.Value;
                        html = html.Replace(factorItemTemplate, "");

                        factorItemTemplate = factorItemTemplate.Replace(@"<script id='" + id + @"' type='x-tmpl-xoffice'>", "");
                        factorItemTemplate = factorItemTemplate.Replace(@"</script>", "");

                        var pattern2 = @"({){2}(.*)(}){2}";
                        var regex2 = new Regex(pattern2);
                        var order = 1;
                        foreach (var item in Factor.FactoreItems)
                        {
                            var itemTemplate = factorItemTemplate;
                            var match2 = regex2.Match(itemTemplate);
                            while (match2.Success)
                            {
                                ProcessFactorItem(ref itemTemplate, match2.Index, match2.Length, match2.Value, item, order);
                                match2 = regex2.Match(itemTemplate);
                            }
                            order++;
                            allList += itemTemplate;
                        }
                    }
                    SetData(ref html, index, length, value, allList);
                }
            }
            catch (Exception ex)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fn = System.IO.Path.Combine(path, "XOffice-Factor-Error.txt");
                System.IO.File.AppendAllText(fn, $"index : {index}{Environment.NewLine}Length : {length}{Environment.NewLine}{value}{Environment.NewLine}{ex}{Environment.NewLine}");
                html = html.Replace(value, "!خطا!");
            }
        }

        private void ProcessFactorItem(ref string itemTemplate, int index, int length, string value, DBACFactorItem item, int order)
        {
            var jsonText = value.Replace("{{", "{");
            jsonText = jsonText.Replace("}}", "}");
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText);
            var name = f(json.Name.ToString());
            if (name == f("ردیف"))
                SetData(ref itemTemplate, index, length, value, order.ToString("N0"));
            else if (name == f("کالا:عنوان"))
                SetData(ref itemTemplate, index, length, value, item.Product.Title);
            else if (name == f("کالا:کد"))
                SetData(ref itemTemplate, index, length, value, item.Product.Code.ToString("D0"));
            else if (name == f("هزینه"))
                SetData(ref itemTemplate, index, length, value, item.Price.ToString("N0"));
            else if (name == f("مقدار"))
                SetData(ref itemTemplate, index, length, value, item.Count.ToString("N0"));
            else if (name == f("جمع"))
                SetData(ref itemTemplate, index, length, value, item.Sum.ToString("N0"));
            else
                SetData(ref itemTemplate, index, length, value, "");
        }

        private void SetData(ref string html, int index, int length, string value, string code)
        {
            html = html.Replace(value, code);
        }

        public string f(string p)
        {
            return HelperConvert.CorrectPersianBug(p);
        }
    }
}

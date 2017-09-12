using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Shipwreck.KokoroIO.SampleApp.ViewModels
{
    public sealed class MessageViewModel : ViewModelBase
    {
        private readonly RoomPageViewModel _Page;
        private readonly Message _Model;

        internal MessageViewModel(RoomPageViewModel page, Message model)
        {
            _Page = page;
            _Model = model;
        }

        public string Avatar => _Model.Avatar;

        public string DisplayName => _Model.DisplayName;

        public DateTime PublishedAt => _Model.PublishedAt;

        public string Content => _Model.Content;

        public bool IsMerged { get; internal set; }

        #region Blocks

        private ReadOnlyCollection<MessageBlock> _Blocks;

        public ReadOnlyCollection<MessageBlock> Blocks
        {
            get
            {
                if (_Blocks == null)
                {
                    var bs = new List<MessageBlock>();

                    try
                    {
                        var html = _Model.Content.Replace("<br>", "<br />");

                        using (var sr = new StringReader(html))
                        using (var xr = XmlReader.Create(sr, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                        {
                            while (xr.Read())
                            {
                                if (xr.NodeType == XmlNodeType.Element && xr.Depth == 0)
                                {
                                    using (var sxr = xr.ReadSubtree())
                                    {
                                        var e = XElement.Load(sxr);

                                        if (Regex.IsMatch(e.Name.LocalName, "^[uo]l$", RegexOptions.IgnoreCase))
                                        {
                                            bs.Add(new MessageList(this, e));
                                        }
                                        else
                                        {
                                            bs.Add(new MessageParagraph(this, e));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        bs.Clear();

                        var ps = Regex.Split(_Model.RawContent, "(\\r|\\r?\\n){2,}");
                        foreach (var p in ps)
                        {
                            if (!string.IsNullOrEmpty(p))
                            {
                                var pvm = new MessageParagraph(this);
                                using (var sr = new StringReader(p))
                                {
                                    for (var l = sr.ReadLine(); l != null; l = sr.ReadLine())
                                    {
                                        if (pvm.Spans.Count > 0)
                                        {
                                            pvm.Spans.Add(new MessageSpan() { Text = Environment.NewLine });
                                        }
                                        pvm.Spans.Add(new MessageSpan()
                                        {
                                            Text = l
                                        });
                                    }
                                }
                                bs.Add(pvm);
                            }
                        }
                    }
                    _Blocks = Array.AsReadOnly(bs.ToArray());
                }
                return _Blocks;
            }
        }

        #endregion Blocks
    }
}
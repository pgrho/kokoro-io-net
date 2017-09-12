using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.KokoroIO.SampleApp.ViewModels
{
    public sealed class RoomPageViewModel : PageViewModelBase
    {
        private readonly Room _Model;

        internal RoomPageViewModel(MainViewModel main, Room model)
            : base(main)
        {
            _Model = model;
        }

        public string Id => _Model.Id;

        public string ChannelName => _Model.ChannelName;

        public RoomKind Kind => _Model.Kind;

        private ObservableCollection<MessageViewModel> _Messages;

        public ObservableCollection<MessageViewModel> Messages
        {
            get
            {
                if (_Messages == null)
                {
                    _Messages = new ObservableCollection<MessageViewModel>();
                    InitMessages();
                }
                return _Messages;
            }
        }

        private async void InitMessages()
        {
            try
            {
                var messages = await Main.Client.GetMessagesAsync(_Model.Id, 60);
                foreach (var m in messages.OrderBy(e => e.Id))
                {
                    var prev = _Messages.LastOrDefault();
                    var vm = new MessageViewModel(this, m);

                    vm.IsMerged = prev?.Avatar == vm.Avatar
                                && prev.DisplayName == vm.DisplayName
                                && vm.PublishedAt < prev.PublishedAt.AddMinutes(3);

                    _Messages.Add(vm);
                }
            }
            catch { }
        }
    }
}
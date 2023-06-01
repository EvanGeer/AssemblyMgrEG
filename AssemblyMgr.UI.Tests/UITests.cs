using AssemblyMgr.UI.Extensions;
using System.ComponentModel;

namespace UnitTests
{
    [TestFixture]
    public class UITests
    {
        class TestVM : INotifyPropertyChanged
        {
            private string _stringData;
            public string StringData
            {
                get => _stringData;
                set => this.Notify(PropertyChanged, () => _stringData = value);
            }

            private string _stringDataWithDependencies;
            public bool DependentProp1 => StringDataWithDependencies == null;
            public bool DependentProp2 => StringDataWithDependencies != null;
            public string StringDataWithDependencies
            {
                get => _stringDataWithDependencies;
                set => this.Notify(PropertyChanged, () => _stringDataWithDependencies = value,
                    alsoNotify: new[] { nameof(DependentProp1), nameof(DependentProp2) });
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }


        [SetUp]
        public void Setup()
        {

        }

        [Test(TestOf = typeof(ViewModelExtensions))]
        public void NotifyPropertyChanged_SingleProp()
        {
            Console.WriteLine("Covers " + nameof(ViewModelExtensions.Notify));
            
            string propertyName = string.Empty;

            var viewModel = new TestVM();
            viewModel.PropertyChanged += (sender, e) => propertyName = e.PropertyName;
            viewModel.StringData = "Some Change";

            Assert.AreEqual(nameof(TestVM.StringData), propertyName);
        }
        [Test(TestOf = typeof(ViewModelExtensions))]
        public void NotifyPropertyChanged_AlsoNotifies()
        {
            Console.WriteLine("Covers " + nameof(ViewModelExtensions.Notify));

            var changedProperties = new List<string>();

            var viewModel = new TestVM();
            viewModel.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);
            viewModel.StringDataWithDependencies = "Some Change";

            var expected = 
                $"{nameof(TestVM.DependentProp1)}," +
                $"{nameof(TestVM.DependentProp2)}," +
                $"{nameof(TestVM.StringDataWithDependencies)}";

            var actual = string.Join(",", changedProperties.OrderBy(x => x));

            Assert.AreEqual(expected, actual);
        }
    }
}
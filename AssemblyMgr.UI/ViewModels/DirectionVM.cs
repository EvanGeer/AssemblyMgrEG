using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssemblyMgr.UI.ViewModels
{
    public class DirectionVM : INotifyPropertyChanged
    {
        public IDirection DataModel { get; set; }
        public DirectionVM(IDirection dataModel)
        {
            DataModel = dataModel;
        }


        public bool TopFrontRight { get => get(); set => set(value); }
        public bool TopFrontLeft { get => get(); set => set(value); }
        public bool TopBackRight { get => get(); set => set(value); }
        public bool TopBackLeft { get => get(); set => set(value); }
        public bool BottomFrontRight { get => get(); set => set(value); }
        public bool BottomFrontLeft { get => get(); set => set(value); }
        public bool BottomBackRight { get => get(); set => set(value); }
        public bool BottomBackLeft { get => get(); set => set(value); }

        public bool N { get => get(); set => set(value); }
        public bool S { get => get(); set => set(value); }
        public bool E { get => get(); set => set(value); }
        public bool W { get => get(); set => set(value); }

        public bool BottomLeft { get => get(); set => set(value); }
        public bool TopRight { get => get(); set => set(value); }
        public bool TopLeft { get => get(); set => set(value); }
        public bool BottomRight { get => get(); set => set(value); }


        private bool get([CallerMemberName] string direction = null)
        {
            return DataModel.Direction == getDirection(direction);
        }


        private void set(bool isChecked, [CallerMemberName] string direction = null)
        {
            DataModel.Direction = getDirection(direction);

            this.Notify(PropertyChanged, new[]
            {
                nameof(TopFrontRight),
                nameof(TopFrontLeft),
                nameof(TopBackRight),
                nameof(TopBackLeft),
                nameof(BottomFrontRight),
                nameof(BottomFrontLeft),
                nameof(BottomBackRight),
                nameof(BottomBackLeft),
                
                nameof(N),
                nameof(S),
                nameof(E),
                nameof(W),

                nameof(BottomLeft),
                nameof(BottomRight),
                nameof(TopLeft),
                nameof(TopRight),
            });
        }

        private Direction3d getDirection([CallerMemberName] string direction = null)
        {
            var directoin3dValues = Enum.GetValues(typeof(Direction3d)).Cast<object>();
            var viewCubeValues = Enum.GetValues(typeof(ViewCubeCorner)).Cast<object>();
            var quadrants = Enum.GetValues(typeof(Quadrant)).Cast<object>();

            var enumValues = directoin3dValues
                      .Union(viewCubeValues)
                      .Union(quadrants)
                      .ToList();

            var result = enumValues.FirstOrDefault(x => x.ToString() == direction);
            var _return = result is null ? Direction3d.None : (Direction3d)result;
            return _return;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}


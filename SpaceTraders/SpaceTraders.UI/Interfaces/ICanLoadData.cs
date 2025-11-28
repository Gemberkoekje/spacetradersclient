using System;

namespace SpaceTraders.UI.Interfaces;

internal interface ICanLoadData
{
    Type DataType { get; }

    void LoadData(object data);

    string? ParentSymbol { get; set; }

    string? Symbol { get; set; }
}

internal interface ICanLoadData<in TData> : ICanLoadData
{
    void ICanLoadData.LoadData(object data) => LoadData((TData)data);

    Type ICanLoadData.DataType => typeof(TData);

    void LoadData(TData data);
}

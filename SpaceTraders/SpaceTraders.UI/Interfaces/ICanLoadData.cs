namespace SpaceTraders.UI.Interfaces;

internal interface ICanLoadData<in TData>
{
    void LoadData(TData data);
}

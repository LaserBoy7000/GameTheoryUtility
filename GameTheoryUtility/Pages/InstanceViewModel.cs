namespace GameTheoryUtility.Pages;

public class InstanceViewModel(PracticeViewModel model, int index)
{
    public PracticeViewModel Model => model;
    public int Index => index;
}
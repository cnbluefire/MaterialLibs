# MaterialLibs
为UWP添加一些炫酷的特效
## Documentation
### RippleStateHelper
![](DemoImages/Ripple.gif)
建议:  
编写控件Style或Template，在LayoutRoot中使用m_helper:RippleHelper。
```
<Grid x:Name="LayoutRoot" 
           m_helper:RippleHelper.RippleDuration="0:0:0.8" 
           m_helper:RippleHelper.RippleColor="{ThemeResource SystemBaseLowColor}" 
           m_helper:RippleHelper.RippleHelperState="Pressed" 
           m_helper:RippleHelper.RippleRadius="30" 
/>
```
注意：
* 设置IsFillEnable后，RippleRadius属性将会失效。
### ParticleCanvas
![](DemoImages/ParticleCanvas.gif)
```
<m_control:ParticleCanvas
                LineColor="DarkGray"
                ParticleColor="Gray"
/>
```
注意：
* 使用Paused属性控制是否绘制。
* 使用IsPointerEnable属性控制粒子是否响应鼠标。
### AnimationHamburgerIcon
![](DemoImages/AnimationHamburgerIcon.gif)
```
<Button x:Name="HamburgerButton" Width="48" Height="48" Padding="0" Margin="10"
        HorizontalAlignment="Left" VerticalAlignment="Top"
        Background="Transparent" Foreground="Black" Click="HamburgerButton_Click">
    <Button.Resources>
        <SolidColorBrush x:Key="ButtonBackgroundPointerOver" Color="Transparent" />
        <SolidColorBrush x:Key="ButtonBackgroundPressed" Color="Transparent" />
        <SolidColorBrush x:Key="ButtonForegroundPointerOver" Color="Black" />
        <SolidColorBrush x:Key="ButtonForegroundPressed" Color="Black" />
    </Button.Resources>
    <Button.Content>
        <m_control:AnimationHamburgerIcon x:Name="anIcon" />
    </Button.Content>
</Button>
```

```
private void HamburgerButton_Click(object sender, RoutedEventArgs e)
{
    mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
    anIcon.IsEnded = mainSplitView.IsPaneOpen;
}
```

### CoreSocialistValuesHelper
```
<Grid m_helper:CoreSocialistValuesHelper.IsCoreSocialistValuesEnable="True" >
</Grid>
```
不多说不多说。

### TipsRectangleHelper
![](DemoImages/TipsRectangleHelper-Pivot.gif)
![](DemoImages/TipsRectangleHelper-ListView.gif)
![](DemoImages/TipsRectangleHelper-GridView.gif)

* 对于ListView，GridView，ListBox，请使用TipsRectangleHelper.TipTargetName属性标记播放动画的元素，例如ListView的BorderBackground，ListBox的PressedBackground，或者自行添加到ListView的TipsRectangle、GridView的BorderBackground等自定义Tips；
* 其他控件请使用TipsRectangleHelper.State控制From到To，动画将从最后一个设置From的元素播放到第一个设置To的元素。
* 详细Style示例请见[App.xaml](Simple/App.xaml)

下面是一个例子：
ListViewItemContainerStyle请使用C:\Program Files (x86)\Windows Kits\10\DesignTime\CommonConfiguration\Neutral\UAP\(SDK版本号)\Generic\generic.xaml中9943行左右的ListViewItemExpanded Style进行自定义。
首先使用默认的BorderBackground或者添加自定义的FocusPipe，并且在VisualTransition中设置Selected到Normal的过渡持续时间达到延时隐藏的效果（根据内部实现方法不同，某些控件不需要设置，如ListBox，请自行实验）：
```
<Style TargetType="ListViewItem" x:Key="ListViewItemWithTipsContainerStyle">
    ...
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="ListViewItem">
                <Grid x:Name="ContentBorder"
                ...
                    <VisualStateManager.VisualStateGroups>
                        <VisualStateGroup x:Name="CommonStates">
                            <VisualStateGroup.Transitions>
                                <VisualTransition From="Selected" To="Normal" GeneratedDuration="0:0:0.001">
                                    <VisualTransition.Storyboard>
                                        <Storyboard>
                                            <ObjectAnimationUsingKeyFrames Storyboard.TargetName="FocusPipe" Storyboard.TargetProperty="(UIElement.Visibility)">
                                                <DiscreteObjectKeyFrame KeyTime="0:0:0.001" Value="Collapsed" />
                                            </ObjectAnimationUsingKeyFrames>
                                        </Storyboard>
                                    </VisualTransition.Storyboard>
                                </VisualTransition>
                            </VisualStateGroup.Transitions>
                            ...
                        </VisualStateGroup>
                    </VisualStateManager.VisualStateGroups>
                <Grid x:Name="ContentPresenterGrid"
                    ...
                </Grid>
                <Rectangle x:Name="FocusPipe" HorizontalAlignment="Left" VerticalAlignment="Center" 
                            Width="6" Height="23" Visibility="Collapsed"
                            Fill="{ThemeResource SystemControlHighlightAccentBrush}" />
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```  
然后在ListView实例添加附加属性
```
<ListView ItemContainerStyle="{StaticResource ListViewItemWithTipsContainerStyle}" ItemsSource="{x:Bind Items}" 
          m_helper:TipsRectangleHelper.TipTargetName="FocusPipe">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Content}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

Pivot请在generic.xaml中搜索
```
<Style TargetType="PivotHeaderItem">
```
然后将相关段落复制到以下两个位置其中之一：
* Pivot控件的Resource中。
* Pivot所在的Page的Resource，或者更上层（例如App.xaml）中。
```
    <Style TargetType="PivotHeaderItem">
    ...
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="PivotHeaderItem">
                    <Grid x:Name="Grid" 
                    ...
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name="SelectionStates">
                                <VisualStateGroup.Transitions>
                                    <VisualTransition From="Unselected" To="UnselectedLocked" GeneratedDuration="0:0:0.33" />
                                    <VisualTransition From="UnselectedLocked" To="Unselected" GeneratedDuration="0:0:0.33" />
                                    <VisualTransition From="Selected" To="Unselected" GeneratedDuration="0:0:0.001" >
                                        <VisualTransition.Storyboard>
                                            <Storyboard>
                                                <ObjectAnimationUsingKeyFrames Storyboard.TargetName="FocusPipe" Storyboard.TargetProperty="(UIElement.Visibility)">
                                                    <DiscreteObjectKeyFrame KeyTime="0:0:0.001" Value="Collapsed" />
                                                </ObjectAnimationUsingKeyFrames>
                                            </Storyboard>
                                        </VisualTransition.Storyboard>
                                    </VisualTransition>
                                </VisualStateGroup.Transitions>
                                ...
                                <VisualState x:Name="Selected">
                                    <Storyboard>
                                    ...
                                    </Storyboard>
                                    <VisualState.Setters>
                                        <Setter Target="FocusPipe.(m_helper:TipsRectangleHelper.State)" Value="To" />
                                    </VisualState.Setters>
                                </VisualState>
                                SelectedPointerOver、SelectedPressed同上。
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                        ...
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```
注意**不要**设置x:Key和x:Name，Style会自动生效。

详细的见Simple/App.xaml，模板中设定TipsRectangleHelper.Token，然后使用VisualState控制TipsRectangleHelper.State
注意：
* 尽量不要为不同控件设置相同的Token。
* 对于ListViewBaseItemContainerStyle等VisualStateGroup中包含Pressed状态的控件，一定要给Pressed的State设置为To，不然动画不会生效。
* 一定要在VisualTransition中，处理Selected到Unselected和Selected到Normal的转换时间，让原Tips晚一段时间消失，动画才能正确播放。
# MaterialLibs
为UWP添加一些炫酷的特效
## Documentation
### RippleStateHelper
建议:  
编写控件Style或Template，在LayoutRoot中使用material:RippleHelper。
```
<Grid x:Name="LayoutRoot" 
           material:RippleHelper.RippleDuration="0:0:0.8" 
           material:RippleHelper.RippleColor="{ThemeResource SystemBaseLowColor}" 
           material:RippleHelper.RippleHelperState="Pressed" 
           material:RippleHelper.RippleRadius="30" 
/>
```
注意：
* 设置IsFillEnable后，RippleRadius属性将会失效。
### ParticleCanvas
```
<material:ParticleCanvas
                LineColor="DarkGray"
                ParticleColor="Gray"
/>
```
注意：
* 使用Paused属性控制是否绘制。
* 使用IsPointerEnable属性控制粒子是否响应鼠标。
### AnimationHamburgerIcon
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
        <material:AnimationHamburgerIcon x:Name="anIcon" />
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
<Grid material:CoreSocialistValuesHelper.IsCoreSocialistValuesEnable="True" >
</Grid>
```
不多说不多说。

### TipsRectangleService
ListViewItemContainerStyle请使用C:\Program Files (x86)\Windows Kits\10\DesignTime\CommonConfiguration\Neutral\UAP\(SDK版本号)\Generic\generic.xaml中9943行左右的ListViewItemExpanded Style进行自定义。
```
<Style TargetType="ListViewItem" 
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
                        <VisualState x:Name="Selected">
                            <VisualState.Setters>
                                <Setter Target="FocusPipe.(material:TipsRectangleService.State)" Value="To" />
                            </VisualState.Setters>
                        </VisualState>
                        PointerOverSelected、PressedSelected、Pressed同上
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            <Grid x:Name="ContentPresenterGrid"
                ...
            </Grid>
            <Rectangle x:Name="FocusPipe" HorizontalAlignment="Left" VerticalAlignment="Center" 
                        Width="6" Height="23" Visibility="Collapsed"
                        Fill="{ThemeResource SystemControlHighlightAccentBrush}"
                        material:TipsRectangleService.State="From"
                        material:TipsRectangleService.Token="ListViewItemWithTips"/>
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
                                        <Setter Target="FocusPipe.(material:TipsRectangleService.State)" Value="To" />
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

详细的见Simple/App.xaml，模板中设定TipsRectangleService.Token，然后使用VisualState控制TipsRectangleService.State
注意：
* 尽量不要为不同控件设置相同的Token。
* 对于ListViewBaseItemContainerStyle等VisualStateGroup中包含Pressed状态的控件，一定要给Pressed的State设置为To，不然动画不会生效。
* 一定要在VisualTransition中，处理Selected到Unselected和Selected到Normal的转换时间，让原Tips晚一段时间消失，动画才能正确播放。
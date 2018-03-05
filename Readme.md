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
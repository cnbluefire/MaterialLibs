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

### VisualHelper
* VisualHelper.Opacity：设置Visual透明度。
* VisualHelper.RotationAngle：设置Visual旋转的弧度值。
* VisualHelper.RotationAngleInDegrees：设置Visual旋转角度值。
* VisualHelper.RotationAxis：设置Visual在三维坐标系中的旋转角度值。
* VisualHelper.Size：设置Visual的大小。不一定生效。
* VisualHelper.Offset：设置Visual相对坐标系的位移，可能会影响到布局。
* VisualHelper.Scale：设置Visual的缩放，缩放中心和AnchorPoint与CenterPoint有关。
* VisualHelper.Clip：设置Visual的Clip，分别是"左,上,右,下"，含义是从边到中心收缩的距离，比如"2,2,2,2"将Visual四个边都裁剪2像素，设置为"0,0,0,0"，Visual内的子元素超出Visual范围将不可见。设置为null将取消Clip。
* VisualHelper.AnchorPoint：设置Visual的锚点，左上角为"0,0"，右下角为"1,1"，可以设置缩放、旋转的基准，但是可能影响布局，推荐使用VisualHelper.CenterPoint。
* 设置VisualHelper.CenterPoint将设置Visual的缩放和旋转基准点，例如设置VisualHelper.CenterPoint="0,50"，缩放将以Visual所在坐标系的(0,50)为中心进行缩放。
* 设置VisualHelper.CenterPoint="Bind"，会将CenterPointer绑定到自身的中心上，也就是Vector3(this.Target.Size.X / 2 ,this.Target.Size.Y / 2 ,0f);

### ImplicitAnimations
* API还在调整，不推荐现在使用。
* Implicit.Animations响应系统Tiggers，例如Target标记为Offset的动画会响应位置改变，标记为Opacity的动画会响应透明度的改变；其他例如标记为Scale，Rotation之类的，需要使用VisualHelper设置对应的Visual属性。
* Implicit.ShowAnimation和HideAnimation分别对应元素出现和消失时的动画（包括加载、卸载、设置Visibility），由于某些未知原因，某些情况下某些动画会无法播放，还请自行实验。

```
    <Grid.Resources>
        <m_easing:CubicBezierEasingFunction x:Key="cbease" ControlPoint="0.17,0.67,0.63,1" />
        <m_animation:AnimationCollection x:Key="ScaleAnimation" >
            <m_animation:Animation AnimationMode="Vector3" Target="Scale" Duration="0:0:1" EasingFunction="{StaticResource cbease}">
                <m_animation:StartingKeyFrame Progress="0"  />
                <m_animation:FinalKeyFrame Progress="1" />
            </m_animation:Animation>
        </m_animation:AnimationCollection>
        <m_animation:AnimationGroup x:Key="ShowAnimation">
            <m_animation:Animation AnimationMode="Vector3" Target="Scale" Duration="0:0:1" EasingFunction="{StaticResource cbease}">
                <m_animation:KeyFrame Progress="0" Value="Vector3(0f,0f,0f)" />
                <m_animation:KeyFrame Progress="0.8" Value="Vector3(1.2f,1.2f,1.2f)" />
                <m_animation:KeyFrame Progress="1" Value="Vector3(1f,1f,1f)" />
            </m_animation:Animation>
            <m_animation:Animation AnimationMode="Scalar" Target="Opacity" Duration="0:0:1" EasingFunction="{StaticResource cbease}">
                <m_animation:KeyFrame Progress="0" Value="0f" />
                <m_animation:KeyFrame Progress="1" Value="1f" />
            </m_animation:Animation>
        </m_animation:AnimationGroup>
        <m_animation:Animation x:Key="HideAnimation" AnimationMode="Scalar" Target="Opacity" Duration="0:0:1">
            <m_animation:KeyFrame Progress="0" Value="1f" />
            <m_animation:KeyFrame Progress="1" Value="0f" />
        </m_animation:Animation>
    </Grid.Resources>
```

```
    <Button x:Name="ShowHideButton" Content="ShowHideButton" 
            m_helper:VisualHelper.CenterPoint="Bind" m_animation:Implicit.Animations="{StaticResource ScaleAnimation}">
    </Button>
    <Rectangle Fill="Red" Width="50" Height="50" m_helper:VisualHelper.CenterPoint="Bind"
            m_animation:Implicit.ShowAnimation="{StaticResource ShowAnimation}" 
            m_animation:Implicit.HideAnimation="{StaticResource HideAnimation}" />
```

# ScrollHeaderPanel
* 使元素与ScrollViewer的滚动数值关联。
* UseQuickBack为True的时候，可能会有掉帧现象，暂时无法解决。
* 要动画的Header**不要**放在ListView.Header里。
* 请在滚动块顶部留下足够的空白，比如ListView的Header设为内容为空，高度等于ScrollHeaderPanel的Border；ScrollViewer内部的面板设置足够的Margin.Top。

```
    <Grid m_helper:VisualHelper.Clip="0,0,0,0">
        <ListView x:Name="ScrollHeaderListView" VerticalAlignment="Stretch" ItemsSource="{x:Bind Items}" SelectionMode="Extended" 
                ScrollViewer.IsVerticalScrollChainingEnabled="True" >
            <ListView.Header>
                <Border Height="200" />
            </ListView.Header>
            <ListView.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Content}" />
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
        <m_control:ScrollHeaderPanel Height="200" VerticalAlignment="Top" HorizontalAlignment="Stretch"
                                    TargetScroller="{x:Bind ScrollHeaderListView}" TargetScrollerName="ScrollViewer" 
                                    Threshold="150" UseQuickBack="{x:Bind UseQuickBackToggle.IsOn,Mode=OneWay}"
                                    OffsetYFrom="0" OffsetYTo="-150" ScrollHeaderStateChanged="ScrollHeaderPanel_ScrollHeaderStateChanged" >
            <Grid Background="#FFDAA72E" m_helper:VisualHelper.Clip="0,0,0,0">
                <m_control:ScrollHeaderPanel VerticalAlignment="Stretch" HorizontalAlignment="Stretch" TargetScroller="{x:Bind ScrollHeaderListView}" TargetScrollerName="ScrollViewer" 
                                            ContentCenterPoint="bind" Threshold="150" UseQuickBack="{x:Bind UseQuickBackToggle.IsOn,Mode=OneWay}"
                                            ScaleFrom="1" ScaleTo="1.1"
                                            OpacityFrom="1" OpacityTo="0">
                    <Image Source="/Assets/imgs/12.jpg" Stretch="UniformToFill" VerticalAlignment="Center" />
                </m_control:ScrollHeaderPanel>
                <Grid Margin="55,0">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>
                    <m_control:ScrollHeaderPanel Height="60" TargetScroller="{x:Bind ScrollHeaderListView}" TargetScrollerName="ScrollViewer" 
                                                Threshold="150" UseQuickBack="{x:Bind UseQuickBackToggle.IsOn,Mode=OneWay}"
                                                OffsetYFrom="0" OffsetYTo="150" 
                                                ScaleFrom="1" ScaleTo="0.9">
                        <TextBlock FontSize="40" Text="This is the First List Title" />
                    </m_control:ScrollHeaderPanel>
                    <m_control:ScrollHeaderPanel Grid.Row="1" Height="40" TargetScroller="{x:Bind ScrollHeaderListView}" TargetScrollerName="ScrollViewer" 
                                                Threshold="150" UseQuickBack="{x:Bind UseQuickBackToggle.IsOn,Mode=OneWay}"
                                                OffsetYFrom="0" OffsetYTo="150" 
                                                ScaleFrom="1" ScaleTo="0.5" 
                                                OpacityFrom="1" OpacityTo="0">
                        <TextBlock FontSize="30" Text="This is the Second List Title" />
                    </m_control:ScrollHeaderPanel>
                </Grid>
            </Grid>
        </m_control:ScrollHeaderPanel>
    </Grid>
```

* TargetScroller：设置绑定到的ScrollViewer或者含有ScrollViewer的元素。不要使用Binding，绑定请使用{x:Bind XXX,Mode=OneWay}。
* TargetScrollerName：标记含有ScrollViewer的元素内部的ScrollViewer的名称，将通过这个名称寻找ScrollViewer。如果要绑定到ScrollViewer本身请不要设置这个属性。
* UseQuickBack：设置为True，将在逆向滚动的时候减小进度而不是回到顶部才修改进度。
* Threshold：设置阈值，动画将以阈值为总长度。
* OffsetYFrom：ScrollHeaderPanel纵向开始的位置，上负下正。
* OffsetYTo：ScrollHeaderPanel纵向最终的位置，上负下正。
* ScaleFrom、ScaleTo、OpacityFrom、OpacityTo同上，注意不要设置为负数。
* ScrollHeaderStateChanged：头部状态改变的事件，尽量不要在这里进行繁重工作，可能会拖慢动画速度。

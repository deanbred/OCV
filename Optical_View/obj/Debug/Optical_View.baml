   M S B A M L   `   `   `  ����G  COptical_View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Optical_ViewOptical_View  5      'clr-namespace:Converters
Converters  5          Optical_View.OpticalView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:Optical_View     5      )convclr-namespace:Converters     5      -   5          � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $M Gpack://application:,,,/Cryoview_Library;component/Cryoview_Library.xaml?�6%   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $	��500 �6   �� 5      .+�5   
   �� 
����İ�._�5      �� -   5        $��
OpticalImg��5   !   $��0��6   $��Uniform=�6V   $	��500��6h   ��63   ��  �� Path$ 
OpticalImg�5   H   5      �� 5    
   -   5        $��
fidRectROI��5       $��0��6&   $��2��64   $��2��6>   
���  ��$��3��6V   z�5!      T� 5"      -   5        fidTTROI��5"   &   5$      `� 5&   
   -   5        $��tlbrtrayOptical��5&      $��1��6-    `� IsLocked . .�5'      c� $	��200��6   $	��Top=�6#   .@�5(      ��  �� Orientation$ 
Horizontal=�6   .+�5)      �� $ Vertical=�6!   $��5q�68   .+�5*      �� $��Exposure��6       lblStyleCaption#��[  63   �� 5+      $	��ROI��6    #��[  6.   �� 5,      -   5         ��Name$ 	SetROIbtn��5,   !   $��Set ROI��64     cmdStyleCaption#��[ 6F   $��26��6�   $	��101��6�   �� 5-      $��
Frames/Sec��6    #��[  65   5.      �� 5/      $ Vertical=�6!   $��5q�68   .+�50      �� -   5         ��Name$ Exposuretxt��50   B   $��3q�6"   	 �� TextAlignment$
	 Left=�6-     txtbxStyleText#��[ 52   "   $��24��6J   $	��103��6V   ��51      �� *
Exposure+
 �� 	Converter
 *  $Converters.ConvertSingleToPrecision1   �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors . 52   V   �� 53      -   5         ��Name$ ROISetIndicatorlbl��53         lblStyleSetting#��[ 6`   $��24��6�   ��6<   �� *ROISetIndicator+6�   �� 54      -	   5        $ RemoveROIbtn��54   !   $��
Remove ROI��67   #��[ 6L   $	��103��6�   $��26��6�   �� 55      -
   5        $ FPSlbl��55   B   $��3q�6"   $
	 Left=�6-   #��[ 57   "   $��24��6J   $	��103��6V   ��56   "   �� *FramesPerSecond+
 #  Converters.ConvertIntToString  $ TwoWay=�$ PropertyChanged=� . 57   V   58      �� 59      $ Vertical=�6!   $��5q�68   .+�5:      �� -   5        $ LiveViewbtn��5:   !   $��	Live View��66   #��[ 6J   $	�� 26��6�   �� 5;      -   5        $ 
LastImgbtn��5;   !   $��
Last Image��65   #��[ 6J   $��26��6�   �� 5<      -   5        $ 
TakeImgbtn��5<   !   $��
Take Image��65   #��[ 6J   $��26��6�   5=      �� 5>      $ Vertical=�6!   $��5q�68   .+�5?      �� $��       Gain��6    #��[  66   �� 5@      -   5        $ Gaintxt��5@   D   $��3q�6"   $	 Center=�6-   #��[ 5A   �   $��22��6�   ��6"   �� *Gain+
   $ TwoWay=�$ PropertyChanged=� . 6�   �� 5B      $ 
Horizontal=�6%   $��5,0,5,0q�6>   .+�5C      �� -   5        $ GainIncreasebtn��5C   %   $��+��6>   #��[ 6J   $��27��6�   $��0q�6�   �� 5D      -   5        $ GainDecreasebtn��5D   %   $��-��6>   #��[ 6J   $��27��6�   $��30,0,0,0q�6�   5E      5F      5G      5H      5I      5K      5L      
   M S B A M L   `   `   `  ����E  AFocus_View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Focus_View
Focus_View  5      'clr-namespace:Converters
Converters  5          Focus_View.FocusView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      *localclr-namespace:Focus_View     5      )convclr-namespace:Converters     5      -   5          � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $M Gpack://application:,,,/Cryoview_Library;component/Cryoview_Library.xaml?�6%   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $	��49* �6   �� 5      $
��221* �6   �� 5      $
��219* �6   �� 5      $	��50* �6   .+�5   
   �� $��4��6   
�������6"   �� 5   
   $��1��6    �� Orientation$ Vertical=�6"   .+�5       �� $��Optical View 1: ��6      lblStyleCaption#��[  6/   �� 5!      $ 
Horizontal=�6   .+�5"      �� $��Last Measured Focus:��6   #��[  67   �� 5#      -   5         ��Name$ currentFocus1lbl��5#        lblStyleSetting#��[ 6X   $	��163��6�   ��62   �� *CurrentFocusView1+6�   5$      �� 5%      $ 
Horizontal=�6   .+�5&      �� -   5         ��Name$ 
setROI1btn��5&      $��Use ROI��6-     cmdStyleCaption#��[ 6?   $	��150��6�   �� 5'      -   5        $ removeROI1btn��5'      $��
Remove ROI��60   #��[ 6E   $	��150��6�   5(      �� 5)      $ 
Horizontal=�6   .+�5*      �� -   5        $ measureFocus1btn��5*      $��Measure Focus��63   #��[ 6K   $	��150��6�   �� 5+      -   5        $ findBestFocus1btn��5+      $��
Auto Focus��64   #��[ 6I   $	��150��6�   �� 5,      -   5        $ cancelFocus1btn��5,      $��Cancel Auto Focus��62   #��[ 6N   $	��150��6�   5-      �� 5.      -   5        $ 
ROISet1lbl��5.      $
��Left=�6)     lblBoolStyleSetting#��[ 6b   $	��150��6�   $��50��6�   ��6D   �� *	IsRIOSet1+6�   5/      �� 51   
   $��2��6   $ Vertical=�6"   .+�52      �� $��Optical View 2: ��6   #��[  6/   �� 53      $ 
Horizontal=�6   .+�54      �� $��Last Measured Focus:��6   #��[  67   �� 55      -	   5        $ currentFocus2lbl��55      #��[ 6X   $	��163��6�   ��62   �� *CurrentFocusView2+6�   56      �� 57      $ 
Horizontal=�6   .+�58      �� -
   5        $ 
setROI2btn��58      $��Use ROI��6-   #��[ 6?   $	��150��6�   �� 59      -   5        $ removeROI2btn��59      $��
Remove ROI��60   #��[ 6E   $	��150��6�   5:      �� 5;      $ 
Horizontal=�6   .+�5<      �� -   5        $ measureFocus2btn��5<      $��Measure Focus��63   #��[ 6K   $	��150��6�   �� 5=      -   5        $ findBestFocus2btn��5=      $��
Auto Focus��64   #��[ 6I   $	��150��6�   �� 5>      -   5        $ cancelFocus2btn��5>      $��Cancel Auto Focus��62   #��[ 6N   $	��150��6�   5?      �� 5@      -   5        $ 
ROISet2lbl��5@      $
��Left=�6)   #��[ 6b   $	��150��6�   $��50��6�   ��6D   �� *	IsRIOSet2+6�   5A      5C      5D      
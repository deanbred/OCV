   M S B A M L   `   `   `  ����M  Ilib_ZoomPanControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS<clr-namespace:lib_ZoomPanControl;assembly=lib_ZoomPanControllib_ZoomPanControl  5   	    Optical_View"   Optical_View.PopoutImageView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Nlocal<clr-namespace:lib_ZoomPanControl;assembly=lib_ZoomPanControl     5   	   -   5        .��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   .+�5      �� -   5        $��
cmdZoomOut��5      $��      -      ��6'   $��5q�6?   6J   �� 5      -   5        $��	cmdZoomIn��5      $��      +      ��6'   $��5q�6?   6J   5      �� 5   
   -   5         ��Name$ 	scrollImg��5      $��1��6*   ��. $��Visible=�5   /   $��Visible=�6U   .��5      '   !lib_ZoomPanControl.ZoomPanControl  -   5          Name$ controlZoomPan��5   #   
�������.��5      ]� -   5         ]�Name$ 	dckpnlImg��5      
�������.+�5      �� -   5         ��Name$ 	LiveImage��5      $
��None=�6o   ��6/   ��  �� Path$ LiveOpticalImg�6o   5      5      5      A� 5   
   -   5        $��Marker��5      $��1��6   
����  �
����  �$��15��6G   $��15��6R    J� Opacity$	 0.8Q�6^   z�5!      T� 5"      -	   5        MarkerTransform��5"   1   5$      5%      5&      
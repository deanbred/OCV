   M S B A M L   `   `   `  ����M  ICryoview_Main_View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Converters
Converters  5   	   R NCryoview_Main_ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Cryoview_Main_ViewModel;assembly=Cryoview_Main_ViewModelCryoview_Main_ViewModel 5   	   #    Cryoview_Main_View.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )convclr-namespace:Converters     5   	   UvmFclr-namespace:Cryoview_Main_ViewModel;assembly=Cryoview_Main_ViewModel     5   	   -   5        $��
mainWindow��5   	     � Title$  Cryoview 100GBAR��5	   	   $	��850��6"   $��1790.5��6/    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $M Gpack://application:,,,/Cryoview_Library;component/Cryoview_Library.xaml?�6%   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $	��35* �6   �� 5      $
��724* �6   �� 5      $	��60* �6    � ColumnDefinitions 5   
   �� 5       $��9* �6   �� 5!      $	��14* �6   �� 5"      $��9* �6   .+�5%   
   o� $��0��6   $��0��6    o� 
IsMainMenu . $��3��6>   .@�5&      m� $��_File��6   .@�5'      m� -   5        $
��Exit��5'      6)   5(      m� 5)      $��_Help��6   .@�5*      m� -   5        $��About��5*      6*   5+      5,      �� 5-   
   -   5        $��tabImageViewOne��5-      $��1��6,   $��0��69   
�������5.      �� 5/   
   -   5        $��tabImageViewTwo��5/      $��1��6,   $��2��69   
�������50      � 52   
   $��1��6   $��1��6    53      �� 54      $	��25* �6    �� 55      $	��73* �6    .+�58      `� -   5        $��tlbrtrayMain��58       `� IsLocked . .�59      c� $	��187��6   $	��Top=�6'   .@�5:      �� 
����  �$��2q�60   $��2q�6D   ._�5;      ��  �� Orientation$ 
Horizontal=�6%   .+�5<      �� $ Vertical=�6)   $��5q�6@   .+�5=   "   �� $��Tempuratures:��6(      lblStyleCaption#��[  6@   �� 5>   "   $��Layering Sphere:��6(   #��[  6C   �� 5?   "   -   5        $��	LSTemplbl��5?   (     lblStyleSetting#��[ 5@   R   ��5?   9   �� 	 �� Path$	 LayeringSphereTemp�
 �� 	Converter
 *  $Converters.ConvertSingleToPrecision3  5@   R   �� 5A   "   $��Cold Finger Extension:��6(   #��[  6I   �� 5B   "   -   5        $��
CFETemplbl��5B   (   #��[ 5C   R   ��5B   :   �� $	 ColdFingerExtTemp�
   5C   R   5D      �� 5E      $ Vertical=�6)   $��5q�6@   .+�5F   "   �� $��
Target ID:��6(   #��[  6=   �� 5G   "   -	   5        $��TIDlbl��5G   (   #��[ 6X   ��66   �� $	 TargetID�6X   �� 5H   "   $	��HDF��6(   #��[  66   �� 5I   "   -
   5         ��Name$ cmdHDFSaveAll��5I   )   $��Save All��6A     cmdStyleCaption#��[ 6U   6~   5J      �� 5K      $ Vertical=�6)   $��5q�6@   .+�5L   "   �� $��Illuminator Channel 1��6(   #��[  6H   �� 5M   "   -   5        $ cmdSetIlluminatorCH1High��5M   )   $
��High��6K   #��[ 6Z   6�   �� 5N   "   -   5        $ cmdSetIlluminatorCH1Low��5N   )   $	��Low��6J   #��[ 6X   6�   �� 5O   "   $��Illuminator Channel 2��6(   #��[  6H   �� 5P   "   -   5        $ cmdSetIlluminatorCH2High��5P   )   $
��High��6K   #��[ 6Z   6�   �� 5Q   "   -   5        $ cmdSetIlluminatorCH2Low��5Q   )   $	��Low��6J   #��[ 6X   6�   5R      ]� 5S      .+�5T   "   �� $	��Top=�6(   $?��9Current Task:                                            ��6=   #��[  6�   �� 5U   "   -   5        $��Tasklbl��5U   c   $��Bottom=�6(   $��Stretch=�6@   #��[ 6�   ��6r   �� $	 TaskMessage�6�   5V      �� 5W      $ Vertical=�6)   .+�5X   "   �� $��Network:               ��6(   #��[  6J   �� 5Y   "   -   5        $��NetworkConnectionlbl��5Y   (   #��[ 5Z   [   ��5Y   D   �� $	 NetworkConnected�
 3  -Converters.ConvertBoolToConnectedNotConnected  5Z   [   5[      5\      5]      5^      5_      �� 5a      -   5        $��tabFunctionView��5a      $��1��60   $��0��6=   5b      �� 5d   
   $��2��6   $��0��6"   $��3��62   $ 
Horizontal=�6F   .+�5e      �� -   5        $��Clear App Status��5e      #��[ 6R   �� 5f      -   5        $��	Statuslbl��5f   7   $��Stretch=�6   #��[ 6p   $
��1645��6�   ��6H   �� $	 StatusMessage�6�   5g      5j      5k      
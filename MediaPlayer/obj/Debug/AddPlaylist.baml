   M S B A M L   `   `   `  ����F  BMediaPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:MediaPlayerMediaPlayer  5          MediaPlayer.AddPlaylist   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +localclr-namespace:MediaPlayer     5      .��5	      � 
����ح�  � RowDefinitions  5
   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��5* �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��Create your playlist��6!   $��20(�6=   
�������$��SemiBold"�6^   $��Center=�6t   $��0,5,0,0q�6�   6�   �� 5   
   -   5         ��Name$ playlistName��5      $��1��6   $��30��65   $	��500��6A    J� IsHitTestVisible .  �� TextWrapping$
 Wrap=�6e   $�� ��6y   $��Center=�6�   
�������6�   �� 5   
   -   5         ��Name$ Placeholder��5   !   $��1��6   $
��Wrap=�66   $��Playlist name��6J   $��Center=�6_   $��Center=�6z   
�������$��15(�6�   
������ ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � y� �� Value$ 	Collapsed=�63    �� Triggers 5      �� 5      	 �� Binding	 6&   �� *Text+
 �� ElementName$
 playlistName�� �� Value  6Y   .a�5      ��  � y�$ Visible=�6;   5      5      5      � 5   
   $��2��6   $��Center=�6!   $��Center=�6>   $	��Top=�6Y   .+�5       �� -   5        $��btnOpenFile��5       ���#6E6D6F$	��120��6W   $��30��6c   
������ .��6�   Select songs6�   5!      �� 5"   
   -   5        $��lbFiles��5"      $��3��6   
�������$	��300��6A   $��Center=�6M   $y�	Collapsed=�6j   �� 5#   
   -   5        $��errorMessage��5#      $��4��6(   
���  ��$��11(�6F   $y�	Collapsed=�6T   6m   �� 5$   
   -   5         ��Name$ btnSavePlaylist��5$      $��4��6*   $��Center=�67   $	��120��6R   $��30��6^   
����������#6E6D6F
������ $��Stretch=�6�   $��Center=�6�   .��6  Save6  5%      5&      
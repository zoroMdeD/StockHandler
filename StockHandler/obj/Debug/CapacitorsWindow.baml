   M S B A M L   `   `   `  ����G  CStockHandler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:StockHandlerStockHandler  5   	   #    StockHandler.CapacitorsWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:StockHandler     5   	     � Title$  CapacitorsWindow��5   	   $	��370��6"   $	��448��6/    � 
ResizeMode$ NoResize=�6;   .��5	      �  � RowDefinitions 5
   
   �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      $	��100 �6   �� 5      .+�5   
   �� $��
PartNumber��6   $��12(�6&   $��Center=�64   $
��Left=�6O   $��16,0,0,0q�6j   �� 5   
   $��Capacity��6   $��12(�6$   $��1��62   $��Center=�6?   $
��Left=�6Z   $��16,0,0,0q�6u   �� 5   
   $��Voltage��6   $��12(�6#   $��2��61   $��Center=�6>   $
��Left=�6Y   $��16,0,0,0q�6t   �� 5   
   $��TCoefficient��6   $��12(�6(   $��3��66   $��Center=�6C   $
��Left=�6^   $��16,0,0,0q�6y   �� 5   
   $
��Size��6   $��12(�6    $��4��6.   $��Center=�6;   $
��Left=�6V   $��16,0,0,0q�6q   �� 5   
   $��Count��6   $��12(�6!   $��5��6/   $��Center=�6<   $
��Left=�6W   $��16,0,0,0q�6r   �� 5   
   $��1��6.   $��Center=�6>   $��Stretch=�6Y   $��	16,0,16,0q�6w   ��6   �� *
PartNumber+6w   �� 5   
   $��1��6,   $��1��6<   $��Center=�6I   $��Stretch=�6d   $��	16,0,16,0q�6�   ��6   �� *
Capacity+6�   �� 5    
   $��1��6+   $��2��6;   $��Center=�6H   $��Stretch=�6c   $��	16,0,16,0q�6�   ��6   �� *	Voltage+6�   �� 5!   
   $��1��60   $��3��6@   $��Center=�6M   $��Stretch=�6h   $��	16,0,16,0q�6�   ��6   �� *TCoefficient+6�   �� 5"   
   $��1��6(   $��4��68   $��Center=�6E   $��Stretch=�6`   $��	16,0,16,0q�6~   ��6   �� *Size+6~   �� 5#   
   $��1��6)   $��5��69   $��Center=�6F   $��Stretch=�6a   $��	16,0,16,0q�6   ��6   �� *Count+6   �� 5%   
   $��Right=�6    �� Orientation$ 
Horizontal=�61   $��6��6J   $��1��6W   $��0,0,16,0q�6g   .+�5&      �� -   5         �� 	IsDefault . $��60��5&   ;   $��22��6I   $��16,8,0,8q�6U   .��6g   OK6k   �� 5'       �� IsCancel . $��60��6%   $��22��63   $��16,8,0,8q�6?   .��6Q   Отмена6Y   5(      5)      5*      
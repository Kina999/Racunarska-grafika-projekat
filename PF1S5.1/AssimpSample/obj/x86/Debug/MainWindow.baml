   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    AssimpSample   AssimpSample.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	   -   5          � Title$  SharpGL WPF Application��5   	   $	��350��6)   $	��525��66   .��5      �  � RowDefinitions 5   
   �� 5	      �� 5
      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      �� 5      .+�5   
      SharpGL.WPF.OpenGLControl  -   5        $��openGLControl��5         DrawFPS .    RenderContextType$	 FBO=�5         	FrameRate$ 50Q�64   $��3��6C   $��0��6T   $��0��6a   $��4��6q   �� 5   
   $��10q�6   $��Center=�6!   $��0��6<   $��4��6I   .+�5      �� $��20(�6   .��6"   Ambijantalna komponenta:6<   ]� 5      $��Center=�6   $��10q�63   .+�5      �� $
��Left=�6   $
��Bold"�6.   .��6@   R:6D   �� 5      $��Right=�6q    �� TextAlignment$ Right=�6�   $��40��6�   ��6   ��  �� ElementName$ slColorR�� �� Path$ Value�	 �� UpdateSourceTrigger$	 PropertyChanged=�6�   �� 5      -   5        $��slColorR��5   �   
 � Maximum$	
 255Q�6B    �� TickPlacement$ BottomRight=�6P    �� TickFrequency$ 20Q�6l    �� IsSnapToTickEnabled . {�6   �� *AnimationNotActive+6�   5      ]� 5       $��Center=�6   $��10q�63   .+�5!      �� $
��Left=�6   $
��Bold"�6.   .��6@   G:6D   �� 5"      $��Right=�6q   $ Right=�6�   $��40��6�   ��6   �� $ slColorG��$ Value�$	 PropertyChanged=�6�   �� 5#      -   5        $��slColorG��5#   �   $	
 255Q�6B   $ BottomRight=�6P   $ 20Q�6l    . {�6   �� *AnimationNotActive+6�   5$      ]� 5&      $��Center=�6   $��10q�63   .+�5'      �� $
��Left=�6   $
��Bold"�6.   .��6@   B:6D   �� 5(      $��Right=�6q   $ Right=�6�   $��40��6�   ��6   �� $ slColorB��$ Value�$	 PropertyChanged=�6�   �� 5)      -   5        $��slColorB��5)   �   $	
 255Q�6B   $ BottomRight=�6P   $ 20Q�6l    . {�6   �� *AnimationNotActive+6�   5*      5+      �� 5,   
   $��10q�6   $��Center=�6!   $��1��6<   $��4��6I   .+�5-      �� $��13(�6   .��6"   Visina rampe:61   ]� 5.      $��Center=�6   $��10q�63   .+�5/      �� -   5        $��slColorRamp��5/   �   $	
 255Q�6B   $ BottomRight=�6P   $ 20Q�6l    . {�6   �� *AnimationNotActive+6�   50      �� 51      $��13(�6   .��6"   Visina zidova:62   ]� 52      $��Center=�6   $��10q�63   .+�53      �� -   5        $��slColorWall��53   �   $	
 255Q�6B   $ BottomRight=�6P   $ 20Q�6l    . {�6   �� *AnimationNotActive+6�   54      55      57      58      
   M S B A M L   `   `   `  ���� B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1      WpfApp1.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:WpfApp1     -   $��window��  � Title$  
MainWindow��$	��710��$
��1240��$��14(����#fafafa � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=�.���  � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��Stretch=�$��Stretch=�$��
10,30,10,0q�$��0��.+��� $2��,Завантажити зображення:��$��14(�� $��Stretch=�$��Stretch=�.+��� -   $��filePath�� �� 
IsReadOnly . $��Center=�$��Center=�$	��250��$��30��|�.   J� IsHitTestVisible .  $��11(��� -   $�� Відкрити ��$��Center=�$��Center=� � Cursor$
 Hand��$��30��$��14(��� -   $��Stretch=�$��Stretch=�$��0,10,0,0q�$��1q����	#FFEEEEEE���	#FFFAFAFA.@��� $��Шифратор��.���� $��Stretch=�$��Stretch=�.+�� $��Stretch=�$��Stretch=�$��0,20,0,0q�.+��� $&�� Розміри блока (N*N):��$��14(�$��Center=��� -   $��5,0,0,0q�$��80��$��25��$��Center=�$��Center=�.@��� $	��256�� �� 
IsSelected . �� $	��128���� $��64���� $��32���� $��16���� $��8��� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $&�� Згенерувати ключ:���� -   $��5,0,0,0q�$	��140��$��25��$��Center=�$��Center=�.@��� $��без паролю�� . �� $��через пароль���� $��
Ключі��$��Center=�$��Center=�$��0,10,0,0q��� -   $��allGen��$,��&Згенерувати для всіх��$	��160��$��5q�$
��Left=�{�.  �� -   $��passGen��
 J� Uid$
 pass��$��Згенерувати��$	��160��$��5q�$
��Left=�$y�	Collapsed=�{�.  �� -	   $��	showTable��$8��2Переглянути таблицю ключів��$	��220��$��5q�$
��Left=�{�.  � $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $��Alpha��$��24(�$
��Bold"�
�������$��Center=��� -
   $��alphaGen��$��Згенерувати��$	��100��$��25��{�.  � -   $��alphaP��$��Stretch=�$��Stretch=�$y�	Collapsed=�{�.  .+�G� -   $��	alphaPass��$ alpha��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -   $��	alphaOpen��$ alpha��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -   $��vAlpha��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q�� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $	��Red��$��24(�$
��Bold"�
���  ��$��Center=��� -   $��redGen��$��Згенерувати��$	��100��$��25��{�.  � -   $
��redP��$��Stretch=�$��Stretch=�$y�	Collapsed=�{�.  .+�G� -   $��redPass��$	 red��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -   $��redOpen��$	 red��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -   $
��vRed��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q�� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $��Green��$��24(�$
��Bold"�
��� � �$��Center=��� -   $��greenGen��$��Згенерувати��$	��100��$��25��{�.  � -   $��greenP��$��Stretch=�$��Stretch=�$y�	Collapsed=�{�.  .+�G� -   $��	greenPass��$ green��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -   $��	greenOpen��$ green��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -   $��vGreen��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q�� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $
��Blue��$��24(�$
��Bold"�
����  �$��Center=��� -   $��blueGen��$��Згенерувати��$	��100��$��25��{�.  � -   $��blueP��$��Stretch=�$��Stretch=�$y�	Collapsed=�{�.  .+�G� -   $��bluePass��$
 blue��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -   $��blueOpen��$
 blue��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -   $��vBlue��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q��� $��Дешифратор��.���� $��Stretch=�$��Stretch=�.+��� $;��5Завантажте ключ розшифровки:��$��14(�� $��Stretch=�$��Stretch=�.+��� -   $��keyPath�� . $��Center=�$��Center=�$	��250��$��25��|�.   .  $��11(��� -   $��openKey��$�� Відкрити ��$��Center=�$��Center=�$
 Hand��$��25��$��14(�{�.  �� -    $��passDecr��$��Stretch=�$��Stretch=�$y�	Collapsed=�$��0,10,0,0q�.+��� -!   $��passDecrGen��$
 pass��$��Згенерувати��$	��160��$��5q�$
��Left=�{�.  � $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $��Alpha��$��24(�$
��Bold"�
�������$��Center=�� -"   $��
alphaDecrP��$��Stretch=�$��Stretch=�{�.  .+�G� -#   $��alphaDecrPass��$ alpha��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -$   $��alphaDecrOpen��$ alpha��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -%   $��
vAlphaDecr��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q�� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $	��Red��$��24(�$
��Bold"�
���  ��$��Center=�� -&   $��redDecrP��$��Stretch=�$��Stretch=�{�.  .+�G� -'   $��redDecrPass��$	 red��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -(   $��redDecrOpen��$	 red��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -)   $��vRedDecr��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q�� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $��Green��$��24(�$
��Bold"�
��� � �$��Center=�� -*   $��
greenDecrP��$��Stretch=�$��Stretch=�{�.  .+�G� -+   $��greenDecrPass��$ green��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -,   $��greenDecrOpen��$ green��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� --   $��
vGreenDecr��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q�� $��Stretch=�$��Stretch=�$��0,10,0,0q�.+��� $
��Blue��$��24(�$
��Bold"�
����  �$��Center=�� -.   $��	blueDecrP��$��Stretch=�$��Stretch=�{�.  .+�G� -/   $��blueDecrPass��$
 blue��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q��� -0   $��blueDecrOpen��$
 blue��$��Center=�$��Center=�$	��250��$��30��$��14(�$��5q�$y�	Collapsed=��� -1   $��	vBlueDecr��$��Center=�$��Center=�$��10,0,0,0q��� $��./assets/visible.png��$��30��$
��Fill=�$��5,0,0,0q��� $��Stretch=�$��Stretch=�$��
10,30,10,0q�$��1��.+�� $��Stretch=�$��Stretch=�.+��� $��Stretch=�$��Stretch=�.+��� $2��,Оригінальне зображення:��$��Center=�$��14(��� 
�������$	��420��$	��420��$��5q�._��� -2   $��	beforeImg��$
��Fill=��� $��Stretch=�$��Stretch=�.+��� $.��(Оброблене зображення:��$��Center=�$��14(��� 
�������$	��420��$	��420��$��5q�._��� -3   $��afterImg��$
��Fill=��� $��ДіЇ��$��Center=�$��Center=�$��0,20,0,0q�� $��Center=�$��Stretch=�$��0,10,0,0q�.+��� -4   $��encrypt��$��Зашифрувати��$	��120��$��35��{�.  �� -5   $��inverse��$��Розшифрувати��$	��120��$��35��$��10,0,0,0q�{�.  �� -6   $��decrypt��$��Розшифрувати��$	��120��$��35��$��10,0,0,0q�{�.  �� -7   $��export��$��Експортувати��$	��120��$��35��$��10,0,0,0q�{�.  �� -8   $��anyChiphers��.+��� $U��OПорівняти з іншими алгоритмами шифрування:��$��Center=�$��Center=�$��0,10,0,0q�� $��Center=�$��Stretch=�$��0,10,0,0q�.+��� -9   $	��des��$	��DES��$	��100��$��25��{�.  �� -:   $	��aes��$	��AES��$	��110��$��25��$��10,0,0,0q�{�.  
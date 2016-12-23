# KCV_LoggerPlugin

各種ログを記録する KanColleViewer プラグインです。



### 導入方法
* 以下のファイルを KanColleViewer の `Plugins` ディレクトリに配置。
    * `LoggerPlugin.dll`
    * `protobuf-net.dll`
    * `Xceed.Wpf.Toolkit.dll`

* dllの「ブロックの解除」がされていないと正しく動作しません。
    * [https://twitter.com/Grabacr07/status/497403215730589696](https://twitter.com/Grabacr07/status/497403215730589696)



### 使用時の注意
* ログ保存先の変更、ログファイルのインポートを行う時は必ず事前にバックアップをとっておいてください！
* 何が起きても気にしない人だけが使ってください。


### ライセンス
* [The MIT License (MIT)](LICENSE)



### 使用ライブラリ

#### [Elemental Annotations - Dualism](https://www.nuget.org/packages/ElementalAnnotations-Dualism)
> zlib
> Copyright (c) 2015 Takeshi KIRIYA
* **ライセンス :** zlib
* **ライセンス全文 :** [license/ElementalAnnotations.txt](license/ElementalAnnotations.txt)
* **備考 :** KanColleWrapper　の依存ライブラリ

#### [Livet](http://ugaya40.hateblo.jp/entry/Livet)
* **ライセンス :** zlib/libpng

#### [KanColleViewer](https://github.com/Grabacr07/KanColleViewer)
> The MIT License (MIT)
> Copyright (c) 2013 Grabacr07
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [license/KanColleViewer.txt](license/KanColleViewer.txt)

#### [StatefulModel](http://ugaya40.hateblo.jp/entry/StatefulModel)
> The MIT License (MIT)
> Copyright (c) 2015 Masanori Onoue
* **用途 :** M-V-Whatever の Model 向けインフラストラクチャ
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [license/StatefulModel.txt](license/StatefulModel.txt)
* **備考 :** MetroTrilithon　の依存ライブラリ

#### [CsvHelper](https://github.com/JoshClose/CsvHelper)
* **ライセンス :** MS-PL and Apache 2.0
* **ライセンス全文 :** [license/CsvHelper.txt](license/CsvHelper.txt)

#### [Extended WPF Toolkit™ Community Edition](http://wpftoolkit.codeplex.com/)
* **ライセンス :** MS-PL
* **ライセンス全文 :** [license/Ms-PL.txt](license/Ms-PL.txt)

#### [log4net](https://logging.apache.org/log4net/)
* **ライセンス :** Apache License Version 2.0
* **ライセンス全文 :** [license/Apache.txt](license/Apache.txt)
* **備考 :** Nekoxy の依存ライブラリ

#### [MetroRadiance](https://github.com/Grabacr07/MetroRadiance)
> The MIT License (MIT)
> Copyright (c) 2014 Manato KAMEYA
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [license/MetroRadiance.txt](license/MetroRadiance.txt)

#### [MetroTrilithon](https://github.com/Grabacr07/MetroTrilithon)
> The MIT License (MIT)
> Copyright (c) 2015 Manato KAMEYA
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [license/MetroTrilithon.txt](license/MetroTrilithon.txt)

#### [Nekoxy](https://github.com/veigr/Nekoxy)
> The MIT License (MIT)
> Copyright (c) 2015 veigr
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [license/Nekoxy.txt](license/Nekoxy.txt)

#### [protobuf-net](https://github.com/mgravell/protobuf-net)
> BSD License
> Copyright (c) 2008 Marc Gravell
* **ライセンス :** BSD License
* **ライセンス全文 :** [license/protobuf-net.txt](license/protobuf-net.txt)

#### [Rx (Reactive Extensions)](https://rx.codeplex.com/)
* **ライセンス :** Apache License Version 2.0
* **ライセンス全文 :** [license/Apache.txt](license/Apache.txt)



### LATEST BUILD 
[![Build status](https://ci.appveyor.com/api/projects/status/5wl22vm5bu4edcwv/branch/master?svg=true)](https://ci.appveyor.com/project/lscyane/kcv-loggerplugin/branch/master) [LoggerPlugin - AppVeyor](https://ci.appveyor.com/project/lscyane/kcv-loggerplugin/branch/master)

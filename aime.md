```
1. Aimeテストサーバについて


テストサーバ用の環境として、次のサーバが存在します。
アクセスにはWANの回線が必要です。


□ テストAimeサービスサイト

https://web-aime-test.naominet.jp/taime/

　テストサーバ用に、新規にSEGAIDを作成したり、作成したSEGAIDにAimeカードを
紐付ける事ができます。アクセスにはベーシック認証が必要となります。



□ テストAimeサーバ管理ページ

https://net-aime-test.naominet.jp/aime/maintenance/p/


　AimeIDとアクセスコード(AimeカードやFeilca携帯に固有のデータ)の
紐付け状態を確認したり、一度登録したものを解除する事ができます。
また、AimeIDのプレイログを確認することができる管理ページです。

利用にはアクセス元IPアドレスの申請とベーシック認証、アカウント申請が必要となります。



2. APM のWebServiceAPIを利用した開発に必要な手順


・まず、上記Aimeサービスサイトから新規にSEGAIDを作成します。
　サイトの案内に従って作成して下さい。

・次に、作成したSEGAIDでAimeサービスサイトにログインし、
　Aimeカードを登録します。

・SEGAIDに対し、Aimeカード(カード,携帯)を3つまで登録できます。

・APM WebService Users Manualの "Aime/SEGA ID 認証API" の章と
　サンプルプログラムを参考に、SEGAID,とパスワードから登録したAimeID一覧を
　取得してください。

・SEGAIDから取得したAimeIDを用いて、APM WebServiceAPIへアクセスすれば
　そのAimeIDのユーザ情報の取得,更新等を行うことができます。

```

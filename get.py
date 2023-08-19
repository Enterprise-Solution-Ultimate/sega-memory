import sys
import zlib
import base64
import requests

if len(sys.argv) != 4:
    print("please put in game_id version serial")
    print("example: python get.py SDED 1.37 A63E01A5243")
    sys.exit()

game_id = sys.argv[1]
ver = sys.argv[2]
serial = sys.argv[3]

print(game_id)
print(ver)
print(serial)

data = f"game_id={game_id}&ver={ver}&serial={serial}"
compressed = zlib.compress(data.encode(), level=-1)
compressed_b64 = base64.b64encode(compressed).decode()

headers = {
    "Pragma": "DFI",
    "User-Agent": "ALL.Net"
}
url = "http://naominet.jp/sys/servlet/DownloadOrder"
response = requests.post(url, data=compressed_b64, headers=headers)

if response.status_code != 200:
    sys.exit("error dl")

response_data = base64.b64decode(response.content)
decompressed = zlib.decompress(response_data).decode()
print(decompressed)

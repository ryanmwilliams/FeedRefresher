----------------------
FeedRefresher - Radio Updater
----------------------
"FeedRefresher.Main.exe" is a program that downloads podcasts (converting them to MP3 if needed from M4A) and then adds them to associated playlists. 

Feeds specifying the "newpodcasts.pls" playlist get included in a most recent releases playlist of top 12.

----------
How to run
----------
You can run this console application as a scheduled task on Windows Server. 

1. compile the application (or get the .zip)
	a. if compiling, zip the contents of the main program's debug directory ("FeedRefresher\FeedRefresher.Main\bin\Debug")
2. move the .zip file to the file location where you want it to run from (example: "C:\my_services" on Windows Server 2012R) and extract it
3. make sure you have a directory called: "C:\MP3s", this is the root directory of where all the files will be downloaded and logged
4. enable write and modify permissions for "NETWORK SERVICE" and "LOCAL SERVICE" on the "C:\MP3s" folder and folder where you placed the unzipped files
5. define your "feeds.json" file in the root of the application
5. create a scheduled task to run "FeedRefresher.Main.exe" (Task Scheduler MMC can schedule tasks, open from the command prompt with: "Taskschd.msc")

NOTES: 

- All files are converted into MP3 to enable SHOUTcast streaming.
- Initial podcast fetches can be time consuming, especially when the source is M4A because it has to be converted to MP3 (192 Kpbs).

---------------------------------
Defining the podcasts to download
---------------------------------
You must set the following and the "feedId" must be unique within the list, it is used in the final .mp3 final name.

Example feed to pull:
```
{
  "feeds": [
    {
      "feedId": 1,
      "maxFilesToDownload": 5,
      "podcastUrl": "http://www.pwop.com/feed.aspx?show=dotnetrocks&filetype=master&tags=ASP.NET%20MVC%2CASP.NET%2CArchitecture%2CAzure%2COpen%20Source%2CXamarin%2CInternet%20of%20Things",
      "destinationDirectory": "C:\\MP3s\\music\\dotnetrocks",
      "targetPlaylistPaths": [
        "C:\\MP3s\\playlist\\dotnetrocks.pls",
        "C:\\MP3s\\playlist\\newpodcasts.pls"
      ]
    }
  ]
}
```

---------
Debugging
---------
Feed Refresher Radio Updater writes to a log file that can be found in: "C:\MP3s\logs". Please look there for all events occuring related to the program.

-------------------------------
Airing playlists with SHOUTcast
-------------------------------
This program enables the automation of a radio station using ShoutCast ("sc_trans.exe" and "sc_serv.exe"). Visit: https://www.shoutcast.com/ for details on ShoutCast. It adds to playlists and takes the newest podcasts and adds them to a special playlist.

After playlists are updated, "sc_trans.exe" needs to restart to pick up the changes. 

By scheduling the restart of "sc_trans.exe" and running Feed Refresher Radio Updater on a recurring basis, an automated podcast based radio station is a reality.

# Auth and Session

In the previous section we succeeded in setting up Create, Update and Delete functionality for albums in the Music Store.
All of these actions are likely to be performed by some kind of shop manager, or administrator.
In fact, `Path` module defines that all the operations are available under "/admin" route.
It would be nice if we could authorize only chosen users to mess with albums in our Store.
That's exactly what we'll do right now.













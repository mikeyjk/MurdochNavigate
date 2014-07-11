for f in ./* ./**/* ; do
	git rm *.meta
	git rm *.csproj
	git rm *.unityproj
	git rm *.sln
	git rm *.suo
	git rm *.user
	git rm *.userprefs
	git rm *.pidb
	git rm *.booproj
done;

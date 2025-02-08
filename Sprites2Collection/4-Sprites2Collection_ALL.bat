FOR /f "tokens=*" %%a IN ('dir /b *.csv') DO (
	4-Sprites2Collection.exe "%%a"
)
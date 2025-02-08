FOR /f "tokens=*" %%a IN ('dir /b *.csv') DO (
	1-Collection2Sprites.exe "%%a"
)
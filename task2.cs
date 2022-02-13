const int PAGE_SIZE = 45, WORD_REPEAT_LIMIT = 100, MAX_WORDS_COUNT = 1_000_000; 



var globalWords = new string[MAX_WORDS_COUNT];
var wordsInfo = new int[MAX_WORDS_COUNT , WORD_REPEAT_LIMIT];

var globalCursor = 0;

var curPage = 1;
var LineCount = 0;
var curLine = "";
var inputStream = new StreamReader("in.txt");

start:
	curLine = inputStream.ReadLine();
	if(curLine == "")
		goto splitEnd;
	else 
		LineCount++;
	
	if(LineCount > PAGE_SIZE)
	{
		curPage++;
		LineCount = 1;
	}

	var lastWordEndIndex = 0;
	var symbolIndex = 0;

	splitStart:
		if(curLine is null)
			goto splitEnd;
		
		if(symbolIndex == curLine?.Length)
		{
			var innerCursor = 0;
			var curWord = curLine[lastWordEndIndex..symbolIndex];
			

			repeatWordCheck:
				if(innerCursor == globalCursor)
					goto insertPageNumber;
		
				if(globalWords[innerCursor] == curWord)	
					goto updatePageNumber;

				innerCursor++;
				goto repeatWordCheck;
	
			
			updatePageNumber:
				var innerPageCursor = 0;
				
				updatePageNumberLoop:
					if(wordsInfo[innerCursor,innerPageCursor] == curPage)
						goto nextLine;
					if(wordsInfo[innerCursor,innerPageCursor] == 0)
					{
						wordsInfo[innerCursor,innerPageCursor] = curPage;
						goto nextLine;
					}
					innerPageCursor++;
					goto updatePageNumberLoop;
				

			insertPageNumber:
				globalWords[globalCursor] = curWord;
				var insertPageNumberIndex = 0;
				
				insertPageNumberLoop:
					if(wordsInfo[globalCursor, insertPageNumberIndex] == 0)
					{
						wordsInfo[globalCursor, insertPageNumberIndex] = curPage;
						goto nextLine;
					}
					if(wordsInfo[innerCursor,insertPageNumberIndex] == curPage) 
					{ 
						goto nextLine;
					}
					insertPageNumberIndex++;
					goto insertPageNumberLoop;

			nextLine:
			globalCursor++;
			goto start;
		}	
		
		if (curLine[symbolIndex] == ' ')
		{
			var innerCursor = 0;
			var curWord = curLine[lastWordEndIndex..symbolIndex];
			

			repeatWordCheck:
				if(innerCursor == globalCursor)
					goto insertPageNumber;
		
				if(globalWords[innerCursor] == curWord)	
					goto updatePageNumber;

				innerCursor++;
				goto repeatWordCheck;
	
			
			updatePageNumber:
				var innerPageCursor = 0;
				
				updatePageNumberLoop:
					if(wordsInfo[innerCursor,innerPageCursor] == curPage)
						goto nextWord;
					if(wordsInfo[innerCursor,innerPageCursor] == 0)
					{
						wordsInfo[innerCursor,innerPageCursor] = curPage;
						goto nextWord;
					}
					innerPageCursor++;
					goto updatePageNumberLoop;
				

			insertPageNumber:
				globalWords[globalCursor] = curWord;
				var insertPageNumberIndex = 0;
				
				insertPageNumberLoop:
					if(wordsInfo[globalCursor, insertPageNumberIndex] == 0)
					{
						wordsInfo[globalCursor, insertPageNumberIndex] = curPage;
						goto nextWord;
					}
					if(wordsInfo[innerCursor,insertPageNumberIndex] == curPage)
					{
						goto nextWord;
					}
					insertPageNumberIndex++;
					goto insertPageNumberLoop;

			nextWord:
			lastWordEndIndex = ++symbolIndex;
			globalCursor++;
			goto splitStart;
		}
		
		if ((int)curLine[symbolIndex] > 0x41 && (int)curLine[symbolIndex] < 0x5A)
		{
			var oldChar_UTF16LE = (int)curLine[symbolIndex];
			var newChar_UTF16LE =  oldChar_UTF16LE + 0x20;
			var newChar = $"{(char)newChar_UTF16LE}";
			
			curLine = curLine[0..symbolIndex] + newChar + curLine[(symbolIndex+1)..curLine.Length];
			symbolIndex++;
			goto splitStart;
		}

		symbolIndex++;
		goto splitStart;



	splitEnd:
	var output = new string[globalCursor];
	var ouputString = "";
	var globalConcatCursor = 0;
	
	concatInfoLoop:

		ouputString = "";
		if(globalConcatCursor == LineCount)
			goto end;

		ouputString += globalWords[globalConcatCursor] + " - ";

		var infoInnerCursor = 0;

		concatInnerLoop:
			if(wordsInfo[globalConcatCursor,infoInnerCursor] != 0)
				{
					ouputString += wordsInfo[globalConcatCursor, infoInnerCursor] + " ";
					infoInnerCursor++;
					goto concatInnerLoop;
				}
			
		output[globalConcatCursor] = $"{ouputString}";
		globalConcatCursor++;
		goto concatInfoLoop;

end:
	File.WriteAllLines("out.txt", output);
	inputStream.Close();

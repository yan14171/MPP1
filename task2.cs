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
	
	if(LineCount % PAGE_SIZE == 0)
	{
		curPage++;
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
			if(curWord.Length < 5)
				goto start;

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
					if(innerPageCursor == WORD_REPEAT_LIMIT - 1)
						goto start;
					if(wordsInfo[innerCursor,innerPageCursor] == curPage)
						goto start;
					if(wordsInfo[innerCursor,innerPageCursor] == 0)
					{
						wordsInfo[innerCursor,innerPageCursor] = curPage;
						goto start;
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
		
		if (((int)curLine[symbolIndex]) < 0x40)
		{
			var innerCursor = 0;
			var curWord = curLine[lastWordEndIndex..symbolIndex];
			if(curWord.Length < 5)
				goto nextWordNotChanged;

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
					if(innerPageCursor == WORD_REPEAT_LIMIT - 1)
						goto nextWordNotChanged;
					if(wordsInfo[innerCursor,innerPageCursor] == curPage)
						goto nextWordNotChanged;
					if(wordsInfo[innerCursor,innerPageCursor] == 0)
					{
						wordsInfo[innerCursor,innerPageCursor] = curPage;
						goto nextWordNotChanged;
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

			nextWordNotChanged:
			lastWordEndIndex = ++symbolIndex;
			goto splitStart;
		}
		
		if ((int)curLine[symbolIndex] >= 0x41 && (int)curLine[symbolIndex] < 0x5A)
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

		int outerCounter = 0;
		var a = new string[globalCursor];
		var c = wordsInfo;
		var b = new int[a.Length];

	int counter = 0;
	fillLoop:
		b[counter] = counter;
		a[counter] = globalWords[counter];
		if(counter++ != b.Length - 1)
		goto fillLoop;

	outerLoop:
	int innerCounter = outerCounter + 1; 
	if(innerCounter == a.Length)
		goto endSort;
	innerLoop:
	
	//COMPARE
		bool compareResult = false;
		string aS = a[outerCounter];
		string bS = a[innerCounter];
		
		var n = aS.Length < bS.Length ? aS.Length : bS.Length;
		int compareCounter = 0;
		compareLoop:
		if(aS[compareCounter] > bS[compareCounter])
		{
			compareResult = true;
			goto compareEnd;
		}
		if(aS[compareCounter] < bS[compareCounter])
		{
			compareResult = false;
			goto compareEnd;
		}
		if(compareCounter++ < n - 1)
		goto compareLoop;
		compareResult = false;
		
		compareEnd:
	//COMPARE END	
		if(compareResult)
		{
	//SWAP
			string temp = "";
			int tempIndex = 0;
	
			temp = a[outerCounter];
			a[outerCounter] = a[innerCounter];
			a[innerCounter] = temp;

			tempIndex = b[outerCounter];
			b[outerCounter] = b[innerCounter];
			b[innerCounter] = tempIndex;
	//SWAP END
		}
		if(innerCounter++ < a.Length - 1)	
			goto innerLoop;
	
	if(outerCounter++ < a.Length - 1)
	goto outerLoop;

endSort:
	var finalWordCounter = 0;
	var output = new string[globalCursor];
	var ouputString = "";
	var globalConcatCursor = 0;
	
	concatInfoLoop:

		ouputString = "";
		if(globalConcatCursor == globalCursor - 1)
			goto end;

		ouputString += a[globalConcatCursor] + " - ";

//FIND INDEX OF INITIAL LIST



var index = 0;
int outputIndexFindCounter = 0;
outputIndexFindLoop:
if (b[outputIndexFindCounter] == globalConcatCursor)
{
	index = outputIndexFindCounter;
	goto concatInner;
}
if (outputIndexFindCounter++ < b.Length - 1)
	goto outputIndexFindLoop;

concatInner:

var infoInnerCursor = 0;
concatInnerLoop:
	if(wordsInfo[index, infoInnerCursor] != 0)
	{
		ouputString += wordsInfo[index, infoInnerCursor] + " ";
		infoInnerCursor++;
		goto concatInnerLoop;
	}
			
	output[finalWordCounter++] = $"{ouputString}";
	globalConcatCursor++;
	goto concatInfoLoop;

end:
	File.WriteAllLines("out.txt", output);
	inputStream.Close();

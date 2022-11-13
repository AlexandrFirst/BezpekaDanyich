$nclude<iostream>
#include<omp.h>
#include<chrono>
#include<vector>
#include<algorithm>
using namespace std;
using namespace std::chrono;

template<typename T>
using matrix = vector<vector<T>>;

int GetRandomNumberInRange(int min, int max)
{
	int range = max - min + 1;
	int num = rand() % range + min;

	return num;
}

void CheckIsOpenMPOn()
{
#ifdef _OPENMP
	cout << "_OPENMP Defined" << endl;
#else
	cout << "_OPENMP UnDefined" << endl;
#endif
}

void displayMatrix(const matrix<int>& m)
{
	for (const auto& row : m)
	{
		for (const auto& column : row)
		{
			cout << column << " ";
		}
		cout << endl;
	}
}

void deleteColumns(matrix<int> m, int startIndex, int n)
{
	auto currentTime = high_resolution_clock::now();

	int columnCount = m[0].size();
	int rowCount = m.size();

	int newArrSize = columnCount;

	if(startIndex < columnCount)
		newArrSize = columnCount - n < 0 ? 0 : columnCount - n;

	matrix<int> result(rowCount, vector<int>(newArrSize));

	if (newArrSize > 0)
	{
		for (int i = 0; i < rowCount; i++)
		{
			int toBorder = startIndex >= columnCount ? columnCount : startIndex;
			int fromBorder = startIndex + n;
			for (int k = 0; k < toBorder; k++)
			{
				result[i][k] = m[i][k];
			}

			for (int k = fromBorder; k < columnCount; k++)
			{
				int t = startIndex + k - fromBorder;

				result[i][t] = m[i][k];
			}
		}
	}

	//displayMatrix(result);

	auto new_currentTime = high_resolution_clock::now();
	auto elapsedTime = duration_cast<nanoseconds>(new_currentTime - currentTime);
	cout << "It took " << elapsedTime.count() << " ns to complete sync func \n";

}


void deleteColumnsAsync(matrix<int> m, int startIndex, int n)
{
	auto currentTime = high_resolution_clock::now();

	int columnCount = m[0].size();
	int rowCount = m.size();

	int newArrSize = columnCount;

	if (startIndex < columnCount)
		newArrSize = columnCount - n < 0 ? 0 : columnCount - n;

	matrix<int> result(rowCount, vector<int>(newArrSize));

	if (newArrSize > 0)
	{

#pragma omp parallel for num_threads(4)
		for (int i = 0; i < rowCount; i++)
		{
			int toBorder = startIndex >= columnCount ? columnCount : startIndex;
			int fromBorder = startIndex + n;
			for (int k = 0; k < toBorder; k++)
			{
				result[i][k] = m[i][k];
			}

			for (int k = fromBorder; k < columnCount; k++)
			{
				int t = startIndex + k - fromBorder;

				result[i][t] = m[i][k];
			}
		}
	}

	//displayMatrix(result);

	auto new_currentTime = high_resolution_clock::now();
	auto elapsedTime = duration_cast<nanoseconds>(new_currentTime - currentTime);
	cout << "It took " << elapsedTime.count() << " ns to complete async func \n";

}




int main()
{

	CheckIsOpenMPOn();

	srand(time(NULL));

	int rowCount = 10000;
	int columnCount = 20000;

	matrix<int> m(rowCount, vector<int>(columnCount, 0));

	for (auto& row : m)
	{
		for (auto& columnElem : row)
		{
			columnElem = GetRandomNumberInRange(1, 5000);
		}
	}

	deleteColumns(m, 4, 9000);
	cout << "-----------------------------------\n";
	deleteColumnsAsync(m, 4, 9000);

	return 0;
}
- src/BusinessLogic

Your application's non-UI layer (ie, stuff you could run in Node)

- src/tests

Tests for your application

- tests/DatabaseTests.fs

Tests that assert the mock data set (to a very limited degree). This was useful in developing the loading/saving utilities

- tests/DataViewTests.fs

A detailed and commented example of setting up a mock database, and asserting the behaviour of an Observable data view before and after a 
business transaction
# How to start
  - cd Path/to/.sln/file
  - docker build -f .\WashTestTask\Dockerfile -t dal_api .
  - docker build -f .\SaleCreationService\Dockerfile -t sale_creation_service .
  - docker-compose up
  - go to http://localhost:8000/index.html

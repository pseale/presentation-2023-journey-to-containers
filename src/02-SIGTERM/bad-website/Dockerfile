FROM ubuntu

WORKDIR /app
COPY ./target/release/bad-website /app

EXPOSE 5001

ENTRYPOINT ["/app/bad-website"]
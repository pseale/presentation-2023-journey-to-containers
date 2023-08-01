// -----------------------------------
// APOLOGY FROM PETER 2023-08-27:
//
// axum has optional support for SIGTERM/graceful shutdown.
// I did not implement graceful shutdown here, but know that
// you could do so, if desired: https://github.com/tokio-rs/axum/blob/main/examples/graceful-shutdown/src/main.rs
// -----------------------------------

use axum::{routing::get, Router};
use std::net::SocketAddr;

#[tokio::main]
async fn main() {
    // Route all requests on "/" endpoint to anonymous handler.
    //
    // A handler is an async function which returns something that implements
    // `axum::response::IntoResponse`.

    // A closure or a function can be used as handler.

    let app = Router::new().route("/slow", get(handler))
    .route("/", get(|| async { "bad website - root /" }))
    .route("/healthz", get(|| async { "healthy" }));

    // Address that server will bind to.
    let addr = SocketAddr::from(([0, 0, 0, 0], 5001));

    // Use `hyper::server::Server` which is re-exported through `axum::Server` to serve the app.
    axum::Server::bind(&addr)
        // Hyper server takes a make service.
        .serve(app.into_make_service())
        .await
        .unwrap();
}

use tokio::time::{sleep, Duration};
async fn handler() -> &'static str {
    // sleep for X seconds - take from querystring param
    // on each second, log the fact you are still waiting
    sleep(Duration::from_millis(4000)).await;

    // when finished, log
    "Hello, world!"
}
var app = angular.module('MessageApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('MessageAppController', ['$rootScope', '$scope', '$http', '$timeout', function ($rootScope, $scope, $http, $timeout) {

    var isPaddleReady = false;

    $scope.refresh = function (paddleTarget) {
        $http({
            method: "GET",
            url: 'api/Message/' + ball.y + '/' + paddleTarget.y + '/' + paddleTarget.height + '?c=' + new Date().getTime(),
            timeout: 777
        })
            .then(
                function mySuccess(data) {
                    isPaddleReady = true;
                    var newY = paddleTarget.y + data.data;
                    if (newY > 0 && newY <= 300) {
                        paddleTarget.y = newY;
                    }
                },
                function onError(response) {
                    isPaddleReady = false;
                    //console.log('Time Out: ' + paddleTarget.name);
                });
    };

    setInterval(() => {
        let pTarget = (ball.x < canvas.width * 0.65) ? leftPaddle : rightPaddle;
        $scope.refresh(pTarget);
    }, 10);

    // select canvas element
    const canvas = document.getElementById("pong");

    // getContext of canvas = methods and properties to draw and do a lot of thing to the canvas
    const ctx = canvas.getContext('2d');

    const ball = {
        x: canvas.width / 2,
        y: canvas.height / 2,
        radius: 10,
        velocityX: 5,
        velocityY: 5,
        speed: 7,
        color: "WHITE"
    };

    const leftPaddle = {
        x: 0,
        y: (canvas.height - 100) / 2,
        width: 10,
        height: 100,
        score: 0,
        color: "WHITE",
        name: "Left Paddle"
    };

    const rightPaddle = {
        x: canvas.width - 10,
        y: (canvas.height - 100) / 2,
        width: 10,
        height: 100,
        score: 0,
        color: "WHITE",
        name: "Right Paddle"
    };

    const net = {
        x: (canvas.width - 2) / 2,
        y: 0,
        height: 10,
        width: 2,
        color: "WHITE"
    };

    // draw a rectangle, will be used to draw paddles
    function drawRect(x, y, w, h, color) {
        ctx.fillStyle = color;
        ctx.fillRect(x, y, w, h);
    }

    // draw circle, will be used to draw the ball
    function drawArc(x, y, r, color) {
        ctx.fillStyle = color;
        ctx.beginPath();
        ctx.arc(x, y, r, 0, Math.PI * 2, true);
        ctx.closePath();
        ctx.fill();
    }

    // when COM or USER scores, we reset the ball
    function resetBall() {
        ball.x = canvas.width / 2;
        ball.y = canvas.height / 2;
        ball.velocityX = -ball.velocityX;
        ball.speed = 7;
    }

    // draw the net
    function drawNet() {
        for (let i = 0; i <= canvas.height; i += 15) {
            drawRect(net.x, net.y + i, net.width, net.height, net.color);
        }
    }

    // draw text
    function drawText(text, x, y) {
        ctx.fillStyle = "#FFF";
        ctx.font = "75px fantasy";
        ctx.fillText(text, x, y);
    }

    // collision detection
    function collision(b, p) {
        p.top = p.y;
        p.bottom = p.y + p.height;
        p.left = p.x;
        p.right = p.x + p.width;

        b.top = b.y - b.radius;
        b.bottom = b.y + b.radius;
        b.left = b.x - b.radius;
        b.right = b.x + b.radius;

        return p.left < b.right && p.top < b.bottom && p.right > b.left && p.bottom > b.top;
    }

    var isLockDirection = false;
    function update() {
        // change the score
        if (ball.x - ball.radius < 0) {
            rightPaddle.score++;
            resetBall();
        } else if (ball.x + ball.radius > canvas.width) {
            leftPaddle.score++;
            resetBall();
        }

        // the ball has a velocity
        ball.x += ball.velocityX;
        ball.y += ball.velocityY;

        // when the ball collides with bottom and top walls we inverse the y velocity.
        if (ball.y - ball.radius <= 0 || ball.y + ball.radius >= canvas.height) {
            if (isLockDirection === false) {
                ball.velocityY = -ball.velocityY;
                isLockDirection = true;
            }
        }
        else {
            isLockDirection = false;
        }

        // we check if the paddle hit the left paddle or the right paddle
        let selectedPaddle = (ball.x + ball.radius < canvas.width / 2) ? leftPaddle : rightPaddle;

        // if the ball hits a paddle
        if (collision(ball, selectedPaddle)) {
            // we check where the ball hits the paddle
            let collidePoint = (ball.y - (selectedPaddle.y + selectedPaddle.height / 2));
            // normalize the value of collidePoint, we need to get numbers between -1 and 1.
            // -selectedPaddle.height/2 < collide Point < selectedPaddle.height/2
            collidePoint = collidePoint / (selectedPaddle.height / 2);

            // when the ball hits the top of a paddle we want the ball, to take a -45degees angle
            // when the ball hits the center of the paddle we want the ball to take a 0degrees angle
            // when the ball hits the bottom of the paddle we want the ball to take a 45degrees
            // Math.PI/4 = 45degrees
            let angleRad = Math.PI / 4 * collidePoint;

            // change the X and Y velocity direction
            let direction = (ball.x + ball.radius < canvas.width / 2) ? 1 : -1;
            ball.velocityX = direction * ball.speed * Math.cos(angleRad);
            ball.velocityY = ball.speed * Math.sin(angleRad);
        }
    }

    // render function, the function that does al the drawing
    function render() {

        // clear the canvas
        drawRect(0, 0, canvas.width, canvas.height, "#000");

        // draw the user score to the left
        drawText(leftPaddle.score, canvas.width / 4, canvas.height / 5);

        // draw the COM score to the right
        drawText(rightPaddle.score, 3 * canvas.width / 4, canvas.height / 5);

        // draw the net
        drawNet();

        // draw the user's paddle
        drawRect(leftPaddle.x, leftPaddle.y, leftPaddle.width, leftPaddle.height, leftPaddle.color);

        // draw the COM's paddle
        drawRect(rightPaddle.x, rightPaddle.y, rightPaddle.width, rightPaddle.height, rightPaddle.color);

        // draw the ball
        drawArc(ball.x, ball.y, ball.radius, ball.color);
    }

    function game() {
        if (isPaddleReady) {
            update();
            render();
        }
    }

    // number of frames per second
    let framePerSecond = 60;

    //call the game function by FPS
    let loop = setInterval(game, 1000 / framePerSecond);

}]);
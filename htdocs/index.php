<!DOCTYPE html>
<html>
<header>
  <link rel="defaultStyle" href="styles.css">
</header>
<body>
  <p>This is a text</p>
  <?php
  // receive information
  $response = file_get_contents('http://jsonplaceholder.typicode.com/posts/');
  // display information raw
  echo '<pre> RES: ' . $response;
    //decode from json
    $response = json_decode($response);
    ?>

    <?php
    error_reporting(E_ALL | E_STRICT);

    $socket = socket_create(AF_INET, SOCK_DGRAM, SOL_UDP);
    socket_bind($socket, '127.0.0.1', 7000);

    $from = '';
    $port = 0;
    socket_recvfrom($socket, $buf, 12, 0, $from, $port);

    echo "Received $buf from remote address $from and remote port $port" . PHP_EOL;
    ?>
  </body>
  </html>

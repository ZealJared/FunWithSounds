<?php
$files = glob('./*.png');
foreach($files as $file)
{
  $path_parts = pathinfo($file);
  $name_parts = explode('_', $path_parts['filename']);
  $new_name = sprintf(
    '%s_%s.%s',
    $name_parts[1],
    $name_parts[0],
    $path_parts['extension']
  );
  rename($path_parts['basename'], $new_name);
}
